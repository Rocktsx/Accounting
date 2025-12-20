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
            var dueDate = endDate.ToDateTime(TimeOnly.MinValue);
            var dueDate1 = endDate.AddDays(-agingDays).ToDateTime(TimeOnly.MinValue);
            var dueDate2 = endDate.AddDays(-agingDays * 2).ToDateTime(TimeOnly.MinValue);
            var dueDate3 = endDate.AddDays(-agingDays * 3).ToDateTime(TimeOnly.MinValue);

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
                            DueDate = grp.Min(item => item.DueDate),
                            OverdueAmount1 = grp.Sum(item => item.DueDate >= dueDate ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount2 = grp.Sum(item => item.DueDate >= dueDate1 && item.DueDate < dueDate ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount3 = grp.Sum(item => item.DueDate >= dueDate2 && item.DueDate < dueDate1 ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount4 = grp.Sum(item => item.DueDate >= dueDate3 && item.DueDate < dueDate2 ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                        });
            var result = await agingQueryable.ToListAsync(cancellationToken); 
            foreach (var item in result)
            {
                item.OverDays = item.DueDate == null || item.DueDate >= dueDate ? 0 : (dueDate - item.DueDate).Value.Days;
            }
            return result;
        }

        public async Task<IEnumerable<AgingSummaryMultipleCurrencyResult>> GetAgingSummaryMultipleCurrencyListAsync(Guid? subSubjectCode,
           DateOnly endDate, int agingDays = 7, AccountTypeTypes category = AccountTypeTypes.Receivable,
           CancellationToken cancellationToken = default)
        {
            var dueDate = endDate.ToDateTime(TimeOnly.MinValue);
            var dueDate1 = endDate.AddDays(-agingDays).ToDateTime(TimeOnly.MinValue);
            var dueDate2 = endDate.AddDays(-agingDays * 2).ToDateTime(TimeOnly.MinValue);
            var dueDate3 = endDate.AddDays(-agingDays * 3).ToDateTime(TimeOnly.MinValue);

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
                            ForeignAmount = grp.Sum(item => item.ForeignAmount * (int)item.DebitorCreditor),
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
                            DueDate = grp.Min(item => item.DueDate),
                            OverdueAmount1 = grp.Sum(item => item.DueDate >= dueDate ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount2 = grp.Sum(item => item.DueDate >= dueDate1 && item.DueDate < dueDate ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount3 = grp.Sum(item => item.DueDate >= dueDate2 && item.DueDate < dueDate1 ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),
                            OverdueAmount4 = grp.Sum(item => item.DueDate >= dueDate3 && item.DueDate < dueDate2 ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0)) : 0),

                            ForeignOutstandingAmount = grp.Sum(item => item.Category == AccountTypeTypes.Receivable ?
                               (item.ForeignAmount > 0 ? item.ForeignAmount : 0)
                               : (item.ForeignAmount < 0 ? -item.ForeignAmount : 0)),
                            ForeignPrepaidDeposit = grp.Sum(item => item.Category == AccountTypeTypes.Receivable ?
                                (item.ForeignAmount < 0 ? item.ForeignAmount : 0)
                                : (item.ForeignAmount > 0 ? -item.ForeignAmount : 0)),
                            ForeignOverdueAmount1 = grp.Sum(item => item.DueDate >= dueDate ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.ForeignAmount > 0 ? item.ForeignAmount : 0)
                                : (item.ForeignAmount < 0 ? -item.ForeignAmount : 0)) : 0),
                            ForeignOverdueAmount2 = grp.Sum(item => item.DueDate >= dueDate1 && item.DueDate < dueDate ?
                                (item.Category == AccountTypeTypes.Receivable ?
                                (item.ForeignAmount > 0 ? item.ForeignAmount : 0)
                                : (item.ForeignAmount < 0 ? -item.ForeignAmount : 0)) : 0),
                            ForeignOverdueAmount3 = grp.Sum(item => item.DueDate >= dueDate2 && item.DueDate < dueDate1 ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.ForeignAmount > 0 ? item.ForeignAmount : 0)
                                : (item.ForeignAmount < 0 ? -item.ForeignAmount : 0)) : 0),
                            ForeignOverdueAmount4 = grp.Sum(item => item.DueDate >= dueDate3 && item.DueDate < dueDate2 ?
                                 (item.Category == AccountTypeTypes.Receivable ?
                                (item.ForeignAmount > 0 ? item.ForeignAmount : 0)
                                : (item.ForeignAmount < 0 ? -item.ForeignAmount : 0)) : 0),
                        });

            var result = await agingQueryable.ToListAsync(cancellationToken);
            foreach (var item in result)
            {
                item.OverDays = item.DueDate == null || item.DueDate >= dueDate ? 0 : (dueDate - item.DueDate).Value.Days;
            }
            return result;
        }

        public async Task<IEnumerable<AgingDetailResult>> GetAgingDetailListAsync(Guid? subSubjectCode,
          DateOnly endDate, AccountTypeTypes category = AccountTypeTypes.Receivable,
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
                            item.Subject.AccountType.Category,
                            item.SubjectId,
                            item.DocNo,
                            item.CurrencyCode
                        })
                        .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
                        .Select(grp => new
                        {
                            SubSubjectCode = grp.Key.CompanyCode,
                            grp.Key.CompanyName,
                            grp.Key.CompanyOtherName,
                            grp.Key.DocNo,
                            grp.Key.CurrencyCode,
                            grp.Key.Category,
                            DueDate = grp.Where(item => item.IsOriginal == true).Select(item => item.DueDate).FirstOrDefault(),
                            NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                            ForeignAmount = grp.Sum(item => item.ForeignAmount * (int)item.DebitorCreditor),
                            VoucherDate = grp.OrderByDescending(item => item.IsOriginal).Select(item => item.Voucher.VoucherDate).FirstOrDefault(),
                            VoucherCode = grp.OrderByDescending(item => item.IsOriginal).Select(item => item.Voucher.Code).FirstOrDefault(),
                        }).Select(item => new AgingDetailResult
                        {
                            SubSubjectCode = item.SubSubjectCode,
                            CompanyName = item.CompanyName,
                            CompanyOtherName = item.CompanyOtherName,
                            DocNo = item.DocNo,
                            CurrencyCode = item.CurrencyCode,
                            DueDate = item.DueDate,
                            VoucherCode = item.VoucherCode,
                            VoucherDate = item.VoucherDate,
                            OutstandingAmount = item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount > 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount < 0 ? -item.NativeAmount : 0),
                            ForeignAmount = item.Category == AccountTypeTypes.Receivable ? item.ForeignAmount : -item.ForeignAmount,
                            PrepaidDeposit = item.Category == AccountTypeTypes.Receivable ?
                                (item.NativeAmount < 0 ? item.NativeAmount : 0)
                                : (item.NativeAmount > 0 ? -item.NativeAmount : 0),
                        });

            return await agingQueryable.ToListAsync(cancellationToken);
        }
    }
}
