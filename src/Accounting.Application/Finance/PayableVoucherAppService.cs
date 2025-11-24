using Accounting.Common;
using Accounting.Finance.PayableVouchers;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance
{
    public class PayableVoucherAppService : VoucherAppService, IPayableVoucherAppService
    {
        public PayableVoucherAppService(IVoucherRepository repository) : base(repository)
        {
            DeletePolicyName = AccountingPermissions.PayableVouchers.Delete;
            GetListPolicyName = AccountingPermissions.PayableVouchers.Default;
            GetPolicyName = AccountingPermissions.PayableVouchers.Default;

            FunctionCode = FunctionCodes.PayableVoucher;
        }
        [Authorize(AccountingPermissions.PayableVouchers.Create)]
        public override Task<VoucherDto> CreateAsync(VoucherCreateDto input)
        {
            input.VoucherType = VoucherType.ReceivableVoucher;
            return base.CreateAsync(input);
        }

        [Authorize(AccountingPermissions.PayableVouchers.Update)]
        public override Task<VoucherDto> UpdateAsync(Guid id, VoucherUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }

        protected override async Task<IQueryable<Voucher>> CreateFilteredQueryAsync(VoucherFilterRequestDto input)
        {
            input.VoucherType = VoucherType.PayableVoucher;
            return await base.CreateFilteredQueryAsync(input);
        }

        [Authorize(AccountingPermissions.PayableVouchers.UpdateStatus)]
        public override Task UpdateStatus(Guid id, VoucherStatus status)
        {
            return base.UpdateStatus(id, status);
        }
        [Authorize(AccountingPermissions.PayableVouchers.Default)]
        public async Task<IEnumerable<VoucherDetailDto>> GenerateDetailsAsync(GeneratePayableDetailRequestDto input)
        {
            using var generator = new PayableVoucherDetailGenerator(
                LazyServiceProvider, input);
            return await generator.GenerateAsync();
        }
        [Authorize(AccountingPermissions.PayableVouchers.Default)]
        public async Task<IEnumerable<PayableDetailDto>> GetPayableDetailsAsync(Guid id)
        {
            if (id.IsEmpty())
            {
                return [];
            }

            var voucher = await GetEntityByIdAsync(id);
            var docNos = voucher.Details.Where((item => item.SubSubjectCode != null))
                .Select(item => item.DocNo).Distinct();
            var debitorId = voucher.Details.FirstOrDefault(item =>
                item.SubSubjectCode != null)?.SubSubjectCode;

            var queryable = await Repository.WithDetailsAsync(item => item.Details);
            queryable = queryable.Where(new NoVoidVoucherSpecification());
            var detailsQueryable = queryable.SelectMany(item => item.Details).
                                Where(obj => obj.SubSubjectCode == debitorId);

            var receivedDic = voucher.Details.GroupBy(item => new { item.DocNo })
                 .Select(grp => new
                 {
                     grp.Key.DocNo,
                     NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                 })
                .ToDictionary(item => item.DocNo,
                item => item.NativeAmount);

            var notReceivedQueryable = detailsQueryable
                .Where(item => item.IsOriginal == false)
               .GroupBy(item => new { item.DocNo })
               .Select(grp => new
               {
                   grp.Key.DocNo,
                   PaidNativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
               });
            var notReceiveList = await AsyncExecuter.ToListAsync(notReceivedQueryable);
            var notReceivedDic = notReceiveList.ToDictionary(
                    item => item.DocNo, item => item.PaidNativeAmount);

            var list = await GetPayableDetailsAsync(detailsQueryable, docNos, notReceivedDic, receivedDic);
            return list;
        }
        [Authorize(AccountingPermissions.PayableVouchers.Default)]
        public async Task<PagedResultDto<PayableDetailDto>> GetPayableDetailsByDebitorAsync(
            PayableDetailByDebitorRequestDto input)
        {
            if (input.DebitorId.IsEmptyOrNull())
            {
                return new PagedResultDto<PayableDetailDto>();
            }

            var queryable = await Repository.WithDetailsAsync(item => item.Details);
            queryable = queryable.Where(new NoVoidVoucherSpecification());

            var detailQueryable = queryable
                .SelectMany(item => item.Details)
                .Where(item => item.SubSubjectCode == input.DebitorId);

            var notReceivedQueryable = detailQueryable
                .GroupBy(item => new { item.DocNo })
                .Where(grp => grp.Sum(item =>
                        item.NativeAmount * (-(int)item.DebitorCreditor)) > 0)
                .Select(grp => new
                {
                    grp.FirstOrDefault(item => item.IsOriginal == true).Voucher.VoucherDate,
                    grp.Key.DocNo,
                    PaidNativeAmount = grp.Sum(item => item.IsOriginal == false ?
                        item.NativeAmount * (int)item.DebitorCreditor : 0),
                });

            var pageQuerable = notReceivedQueryable
                .OrderBy(item => item.VoucherDate)
                .ThenBy(item => item.DocNo)
               .Skip(input.SkipCount)
               .Take(input.MaxResultCount);

            var count = await AsyncExecuter.CountAsync(notReceivedQueryable);
            var notReceiveList = await AsyncExecuter.ToListAsync(notReceivedQueryable);
            var docNos = notReceiveList.Select(item => item.DocNo);

            var notReceivedDic = notReceiveList.ToDictionary(
                    item => item.DocNo, item => item.PaidNativeAmount);
            var list = await GetPayableDetailsAsync(detailQueryable, docNos, notReceivedDic, []);

            return new PagedResultDto<PayableDetailDto>(count, [.. list]);
        }
        private async Task<IEnumerable<PayableDetailDto>> GetPayableDetailsAsync(
            IQueryable<VoucherDetail> queryable, IEnumerable<string> docNos,
            Dictionary<string, decimal> totalReceiveds, Dictionary<string, decimal> currentReceiveds)
        {
            var detailsQueryable = queryable.
                                    Where(obj => docNos.Contains(obj.DocNo)
                                        && obj.IsOriginal == true)
                                    .Select(item => new { item, item.Voucher.VoucherDate });

            var details = await AsyncExecuter.ToListAsync(detailsQueryable);

            var subjectIds = details.Select(item => item.item.SubjectId).Distinct();
            var subjectDic = await GetSubjectsAsync(subjectIds);

            var roundScale = AccountingCommonConsts.AmountRoundScale;
            var list = details.Select(obj =>
            {
                var item = obj.item;
                totalReceiveds.TryGetValue(item.DocNo, out decimal totalPaidNativeAmount);
                currentReceiveds.TryGetValue(item.DocNo, out decimal currentPaidNativeAmount);

                var hasSubject = subjectDic.TryGetValue(item.SubjectId,
                    out var subject);
                var totalPaidAmount = item.CurrencyRate == 0 ? 0 : Math.Round(totalPaidNativeAmount /
                                    item.CurrencyRate, roundScale);
                var currentPaidAmount = item.CurrencyRate == 0 ? 0 : Math.Round(currentPaidNativeAmount /
                                    item.CurrencyRate, roundScale);

                return new PayableDetailDto
                {
                    SourceId = item.Id,
                    SubjectId = item.SubjectId,
                    DocNo = item.DocNo,
                    VoucherDate = obj.VoucherDate,
                    ForeignAmount = item.ForeignAmount,
                    DebitorCreditor = item.DebitorCreditor,
                    CurrencyCode = item.CurrencyCode,
                    CurrencyRate = item.CurrencyRate,
                    NativeAmount = item.NativeAmount,
                    PaidAmount = totalPaidAmount - currentPaidAmount,
                    PaidNativeAmount = totalPaidNativeAmount - currentPaidNativeAmount,
                    CurrentPaid = currentPaidAmount,
                    NativeCurrentPaid = currentPaidNativeAmount,
                    SubjectCategoryCode = null,
                    AccType = hasSubject == true ?
                        subject.AccountType.Code : string.Empty,
                    AccTypeCategory = hasSubject == true ?
                        subject.AccountType.Category : AccountTypeTypes.Normal,
                    OsAmount = item.ForeignAmount - totalPaidAmount,
                    DueDate = item.DueDate
                };
            });
            return list;
        }
    }
}
