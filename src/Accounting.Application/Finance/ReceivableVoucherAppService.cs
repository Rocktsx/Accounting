using Accounting.Finance.ReceivableVouchers;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance
{
    public class ReceivableVoucherAppService : VoucherAppService, IReceivableVoucherAppService
    {
        public ReceivableVoucherAppService(IVoucherRepository repository) : base(repository)
        {
        }
        /// <summary>
        /// 通过客户id获取收款明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ReceivableDetailDto>>
            GetReceivableDetailsByDebitorAsync(ReceivableDetailsByDebitorRequestDto input)
        {
            if (input.DebitorId == null || Guid.Empty.Equals(input.DebitorId))
            {
                return new PagedResultDto<ReceivableDetailDto>();
            }

            var queryable = await Repository.WithDetailsAsync(item => item.Details);
            queryable = queryable.Where(new NoVoidVoucherSpecification());

            var detailQueryable = queryable
                .SelectMany(item => item.Details)
                .Where(item => item.SubSubjectCode == input.DebitorId);

            var notReceivedQueryable = detailQueryable
                .GroupBy(item => new { item.DocNo })
                .Where(grp => grp.Sum(item =>
                        item.NativeAmount * (int)item.DebitorCreditor) > 0)
                .Select(grp => new
                {
                    grp.FirstOrDefault(item => item.IsOriginal == true).Voucher.VoucherDate,
                    grp.Key.DocNo,
                    PaidNativeAmount = grp.Sum(item => item.IsOriginal == false ?
                        item.NativeAmount * (-(int)item.DebitorCreditor) : 0),
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
            var list = await GetReceivableDetailsAsync(detailQueryable, docNos, notReceivedDic);

            return new PagedResultDto<ReceivableDetailDto>(count, list.ToList());
        }
        /// <summary>
        /// 通过传票id获取收款明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ReceivableDetailDto>>
            GetReceivableDetailsAsync(Guid id)
        {
            if (Guid.Empty.Equals(id))
            {
                return new List<ReceivableDetailDto>(0);
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

            var receivedDic = voucher.Details.ToDictionary(item => item.DocNo,
                item => item.NativeAmount);

            var list = await GetReceivableDetailsAsync(detailsQueryable, docNos,
                receivedDic);
            return list;
        }
        private async Task<IEnumerable<ReceivableDetailDto>> GetReceivableDetailsAsync(
            IQueryable<VoucherDetail> queryable, IEnumerable<string> docNos,
            Dictionary<string, decimal> receiveds)
        {
            var detailsQueryable = queryable.
                                    Where(obj => docNos.Contains(obj.DocNo)
                                        && obj.IsOriginal == true)
                                    .Select(item => new { item, item.Voucher.VoucherDate });

            var details = await AsyncExecuter.ToListAsync(detailsQueryable);

            var subjectIds = details.Select(item => item.item.SubjectId).Distinct();
            var subjectRepository = LazyServiceProvider.GetRequiredService<ISubjectRepository>();
            var subjectQueryable = await subjectRepository.WithDetailsAsync(item => item.AccountType);
            var subjectQuery = subjectQueryable.Where(item => subjectIds.Contains(item.Id));
            var subjects = await AsyncExecuter.ToListAsync(subjectQuery);
            var subjectDic = subjects.ToDictionary(item => item.Id, item => item);

            var list = details.Select(obj =>
            {
                var item = obj.item;
                decimal paidNativeAmount = 0m;
                receiveds.TryGetValue(item.DocNo, out paidNativeAmount);
                var hasSubject = subjectDic.TryGetValue(item.SubjectId,
                    out var subject);
                return new ReceivableDetailDto
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
                    PaidAmount = Math.Round(paidNativeAmount /
                                    item.CurrencyRate, 2),
                    PaidNativeAmount = paidNativeAmount,
                    CurrentPaid = 0,
                    NativeCurrentPaid = 0,
                    SubjectCategoryCode = null,
                    AccType = hasSubject == true ?
                        subject.AccountType.Code : string.Empty,
                    AccTypeCategory = hasSubject == true ?
                        subject.AccountType.Category : AccountTypeTypes.Normal,
                    OsAmount = item.ForeignAmount - Math.Round(
                        paidNativeAmount / item.CurrencyRate, 2),
                    DueDate = item.DueDate
                };
            });
            return list;
        }
    }
}
