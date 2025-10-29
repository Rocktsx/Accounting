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
        /// 获取收款明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ReceivableDetailDto>>
            GetReceivableDetailsByDebitor(ReceivableDetailsByDebitorRequestDto input)
        {
            if (input.DebitorId == null || Guid.Empty.Equals(input.DebitorId))
            {
                return new PagedResultDto<ReceivableDetailDto>();
            }

            var queryable = await Repository.WithDetailsAsync(item =>
                item.Details.Where(obj => obj.SubSubjectCode == input.DebitorId));
            queryable = queryable.Where(new NoVoidVoucherSpecification());

            var notReceivedQueryable = queryable
                .SelectMany(item => item.Details)
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
            var detailQueryable = queryable
                .SelectMany(item => item.Details)
                 .Where(item => item.IsOriginal == true &&
                        docNos.Contains(item.DocNo));
            var details = await AsyncExecuter.ToListAsync(detailQueryable);

            var subjectIds = details.Select(item => item.SubjectId).Distinct();
            var subjectRepository = LazyServiceProvider.GetRequiredService<ISubjectRepository>();
            var subjectQueryable = await subjectRepository.WithDetailsAsync(item => item.AccountType);
            var subjectQuery = subjectQueryable.Where(item => subjectIds.Contains(item.Id));
            var subjects = await AsyncExecuter.ToListAsync(subjectQuery);
            var subjectDic = subjects.ToDictionary(item => item.Id, item => item);

            var notReceivedDic = notReceiveList.ToDictionary(
                    item => item.DocNo, item => item);
            var list = details.Select(item =>
            {
                notReceivedDic.TryGetValue(item.DocNo, out var received);
                var hasSubject = subjectDic.TryGetValue(item.SubjectId, out var subject);
                return new ReceivableDetailDto
                {
                    SourceId = item.Id,
                    SubjectId = item.SubjectId,
                    DocNo = item.DocNo,
                    VoucherDate = received.VoucherDate,
                    ForeignAmount = item.ForeignAmount,
                    DebitorCreditor = item.DebitorCreditor,
                    CurrencyCode = item.CurrencyCode,
                    CurrencyRate = item.CurrencyRate,
                    NativeAmount = item.NativeAmount,
                    PaidAmount = Math.Round(received.PaidNativeAmount /
                                    item.CurrencyRate, 2),
                    PaidNativeAmount = received.PaidNativeAmount,
                    CurrentPaid = 0,
                    NativeCurrentPaid = 0,
                    SubjectCategoryCode = null,
                    AccType = hasSubject == true ? subject.AccountType.Code : string.Empty,
                    AccTypeCategory = hasSubject == true ? subject.AccountType.Category : AccountTypeTypes.Normal,
                    OsAmount = item.ForeignAmount - Math.Round(
                        received.PaidNativeAmount / item.CurrencyRate, 2),
                    DueDate = item.DueDate.Value
                };
            }).ToList();

            return new PagedResultDto<ReceivableDetailDto>(count, list);
        }
    }
}
