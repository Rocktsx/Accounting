using Accounting.EntityFrameworkCore;
using Accounting.Finance.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.Finance
{
    internal class ReceivablePayableAgingReportRepository : VoucherRepository, IReceivablePayableAgingReportRepository
    {
        public ReceivablePayableAgingReportRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<IEnumerable<AgingSummarySingleCurrencyResult>> GetAgingSummarySingleCurrencyListAsync(
            Guid? subSubjectCode, DateOnly endDate, int agingDays = 7, AccountTypeTypes category = AccountTypeTypes.Receivable,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync();
            var agingQueryable = queryable.Where(item => item.VoucherDate <= endDate)
                        .SelectMany(item => item.Details)
                        .WhereIf(subSubjectCode != null, item => item.SubSubjectCode == subSubjectCode)
                        .Where(item => item.Subject.AccountType.Category == category)
                        .GroupBy(item => new
                        {
                            item.SubSubjectCode,
                            CompanyCode = item.Company.Code,
                            CompanyName = item.Company.Name,
                            CompanyOtherName = item.Company.OtherName,
                            item.SubjectId,
                            item.DocNo,
                            item.Subject.AccountType.Category
                        })
                        .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
                        .Select(grp => new
                        {
                            SubSubjectCode = grp.Key.CompanyCode,
                            grp.Key.CompanyCode,
                            grp.Key.CompanyName,
                            grp.Key.CompanyOtherName,
                            grp.Key.SubjectId,
                            grp.Key.DocNo,
                            grp.Key.Category,
                            DueDate = grp.Where(item => item.IsOriginal == true).Select(item => item.DueDate).FirstOrDefault(),
                            NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                            OverDays = grp.Max(item => item.DueDate == null ? 0 : EF.Functions.DateDiffDay(endDate.ToDateTime(TimeOnly.MinValue), item.DueDate.Value))
                        })
                        .GroupBy(item => new
                        {
                            item.SubSubjectCode,
                            item.CompanyCode,
                            item.CompanyName,
                            item.CompanyOtherName
                        })
                        .Select(grp => new AgingSummarySingleCurrencyResult
                        {
                            SubSubjectCode = grp.Key.SubSubjectCode,
                            CompanyName = grp.Key.CompanyName,
                            CompanyOtherName = grp.Key.CompanyOtherName,
                            OutstandingAmount = grp.Sum(item => item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)),
                            PrepaidDeposit = grp.Sum(item => item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount < 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount > 0 ? -item.NativeAmount : 0)),
                            OverDays = (int)grp.Max(item => item.OverDays),
                            OverdueAmount1 = grp.Sum(item => item.OverDays <= 0 ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount2 = grp.Sum(item => item.OverDays <= agingDays && item.OverDays > 1 ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount3 = grp.Sum(item => item.OverDays <= agingDays * 2 && item.OverDays > agingDays ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount4 = grp.Sum(item => item.OverDays <= agingDays * 3 && item.OverDays > agingDays * 2 ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                        });

            return await agingQueryable.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<AgingSummaryMultipleCurrencyResult>> GetAgingSummaryMultipleCurrencyListAsync(Guid? subSubjectCode,
           DateOnly endDate, int agingDays = 7, AccountTypeTypes category = AccountTypeTypes.Receivable,
           CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync();
            var agingQueryable = queryable.Where(item => item.VoucherDate <= endDate)
                        .SelectMany(item => item.Details)
                        .WhereIf(subSubjectCode != null, item => item.SubSubjectCode == subSubjectCode)
                        .Where(item => item.Subject.AccountType.Category == category)
                        .GroupBy(item => new
                        {
                            item.SubSubjectCode,
                            CompanyCode = item.Company.Code,
                            CompanyName = item.Company.Name,
                            CompanyOtherName = item.Company.OtherName,
                            item.SubjectId,
                            item.DocNo,
                            item.Subject.AccountType.Category,
                            item.CurrencyCode,
                            item.CurrencyRate
                        })
                        .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
                        .Select(grp => new
                        {
                            SubSubjectCode = grp.Key.CompanyCode,
                            grp.Key.CompanyCode,
                            grp.Key.CompanyName,
                            grp.Key.CompanyOtherName,
                            grp.Key.SubjectId,
                            grp.Key.DocNo,
                            grp.Key.Category,
                            grp.Key.CurrencyCode,
                            grp.Key.CurrencyRate,
                            DueDate = grp.Where(item => item.IsOriginal == true).Select(item => item.DueDate).FirstOrDefault(),
                            NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                            OverDays = grp.Max(item => item.DueDate == null ? 0 : EF.Functions.DateDiffDay(endDate.ToDateTime(TimeOnly.MinValue), item.DueDate.Value))
                        })
                        .GroupBy(item => new
                        {
                            item.SubSubjectCode,
                            item.CompanyCode,
                            item.CompanyName,
                            item.CompanyOtherName,
                            item.CurrencyCode,
                            item.CurrencyRate
                        })
                        .Select(grp => new AgingSummaryMultipleCurrencyResult
                        {
                            SubSubjectCode = grp.Key.SubSubjectCode,
                            CompanyName = grp.Key.CompanyName,
                            CompanyOtherName = grp.Key.CompanyOtherName,
                            CurrencyCode = grp.Key.CurrencyCode,
                            CurrencyRate = grp.Key.CurrencyRate,
                            OutstandingAmount = grp.Sum(item => item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)),
                            PrepaidDeposit = grp.Sum(item => item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount < 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount > 0 ? -item.NativeAmount : 0)),
                            OverDays = (int)grp.Max(item => item.OverDays),
                            OverdueAmount1 = grp.Sum(item => item.OverDays <= 0 ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount2 = grp.Sum(item => item.OverDays <= agingDays && item.OverDays > 1 ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount3 = grp.Sum(item => item.OverDays <= agingDays * 2 && item.OverDays > agingDays ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount4 = grp.Sum(item => item.OverDays <= agingDays * 3 && item.OverDays > agingDays * 2 ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                        });

            return await agingQueryable.ToListAsync(cancellationToken);
        }
    }
}
