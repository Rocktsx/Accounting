using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance
{
    public class GeneralLedgerReportRepository : IGeneralLedgerReportRepository
    {
        private readonly IQueryable<Voucher> _voucherQueryable;

        /// <summary>
        /// Assets, Liabilities, Capital group
        /// </summary>
        private readonly AccountTypeGroup[] _alcGroups = [AccountTypeGroup.Assets, 
            AccountTypeGroup.Liabilities, AccountTypeGroup.Capital] ;
        public GeneralLedgerReportRepository(IQueryable<Voucher> voucherQueryable)
        {
            _voucherQueryable = voucherQueryable;
        } 

        public async Task<IEnumerable<GeneralLedgerSingleCurrencyReportResult>> GetGLSingleCurrencyListAsync(
            DateOnly startDate, DateOnly endDate,
            DateOnly periodStartDate, DateOnly periodEndDate,
            Guid? subjectId = null ,CancellationToken cancellationToken = default)
        {  
            var lastYearBfQuery = _voucherQueryable.Where(item => item.VoucherDate < periodStartDate)
                                            .SelectMany(item => item.Details)
                                            .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                                            .Where(item => _alcGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
                                            .GroupBy(item => new { 
                                                item.SubjectId , 
                                                SubjectCode = item.Subject.Code,
                                                SubjectName = item.Subject.Name,
                                                SubjectOtherName = item.Subject.OtherName
                                            })
                                            .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
                                            .Select(grp => new GeneralLedgerSingleCurrencyReportResult { 
                                                SubjectCode = grp.Key.SubjectCode,
                                                SubjectName = grp.Key.SubjectName,
                                                SubjectOtherName = grp.Key.SubjectOtherName,
                                                NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                                                SortOrder = AccountingCommonConsts.LastYearBfOrder,
                                                VoucherCode = AccountingCommonConsts.SystemCodeText,
                                                DocNo = string.Empty,
                                                Description = AccountingCommonConsts.LastYearBfText,
                                                VoucherDate = periodStartDate
                                            });

            var currentYearBfQuery = _voucherQueryable.Where(item => item.VoucherDate >= periodStartDate && item.VoucherDate < startDate)
                                            .SelectMany(item => item.Details)
                                            .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                                            .GroupBy(item => new {
                                                item.SubjectId,
                                                SubjectCode = item.Subject.Code,
                                                SubjectName = item.Subject.Name,
                                                SubjectOtherName = item.Subject.OtherName
                                            })
                                            .Select(grp => new GeneralLedgerSingleCurrencyReportResult
                                            {
                                                SubjectCode = grp.Key.SubjectCode,
                                                SubjectName = grp.Key.SubjectName,
                                                SubjectOtherName = grp.Key.SubjectOtherName,
                                                NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                                                SortOrder = AccountingCommonConsts.CurrentYearBfOrder,
                                                VoucherCode = AccountingCommonConsts.SystemCodeText,
                                                DocNo = string.Empty,
                                                Description = AccountingCommonConsts.CurrentYearBfText,
                                                VoucherDate = startDate
                                            });

            var currentQuery = _voucherQueryable.Where(item => item.VoucherDate >= startDate && item.VoucherDate <= endDate )
                                            .SelectMany(item => item.Details)
                                            .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                                            .Select(item => new GeneralLedgerSingleCurrencyReportResult
                                            {
                                                SubjectCode = item.Subject.Code,
                                                SubjectName = item.Subject.Name,
                                                SubjectOtherName = item.Subject.OtherName,
                                                NativeAmount = item.NativeAmount * (int)item.DebitorCreditor,
                                                SortOrder = AccountingCommonConsts.CurrentPeriodOrder,
                                                VoucherCode = item.Voucher.Code,
                                                DocNo = item.DocNo,
                                                Description = item.Description,
                                                VoucherDate = item.Voucher.VoucherDate
                                            });
            var finalQuery = lastYearBfQuery
                                .Concat(currentYearBfQuery)
                                .Concat(currentQuery)
                                .OrderBy(item => item.SubjectCode)
                                .ThenBy(item => item.SortOrder)
                                .ThenBy(item => item.VoucherDate)
                                .ThenBy(item => item.VoucherCode); 

           return await finalQuery.ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GeneralLedgerMultipleCurrencyReportResult>> GetGLMultipleCurrencyListAsync(DateOnly startDate, DateOnly endDate, DateOnly periodStartDate, DateOnly periodEndDate, Guid? subjectId = null, CancellationToken cancellationToken = default)
        {
            var lastYearBfQuery = _voucherQueryable.Where(item => item.VoucherDate < periodStartDate)
                                            .SelectMany(item => item.Details)
                                            .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                                            .Where(item => _alcGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
                                            .GroupBy(item => new {
                                                item.SubjectId,
                                                item.CurrencyCode,
                                                SubjectCode = item.Subject.Code,
                                                SubjectName = item.Subject.Name,
                                                SubjectOtherName = item.Subject.OtherName
                                            })
                                            .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
                                            .Select(grp => new GeneralLedgerMultipleCurrencyReportResult
                                            {
                                                SubjectCode = grp.Key.SubjectCode,
                                                SubjectName = grp.Key.SubjectName,
                                                SubjectOtherName = grp.Key.SubjectOtherName,
                                                NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                                                SortOrder = AccountingCommonConsts.LastYearBfOrder,
                                                VoucherCode = AccountingCommonConsts.SystemCodeText,
                                                DocNo = string.Empty,
                                                Description = AccountingCommonConsts.LastYearBfText,
                                                VoucherDate = periodStartDate,
                                                CurrencyCode = grp.Key.CurrencyCode,
                                                ForeignAmount = grp.Sum(item => item.ForeignAmount * (int)item.DebitorCreditor),
                                            });

            var currentYearBfQuery = _voucherQueryable.Where(item => item.VoucherDate >= periodStartDate && item.VoucherDate < startDate)
                                            .SelectMany(item => item.Details)
                                            .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                                            .GroupBy(item => new {
                                                item.SubjectId,
                                                item.CurrencyCode,
                                                SubjectCode = item.Subject.Code,
                                                SubjectName = item.Subject.Name,
                                                SubjectOtherName = item.Subject.OtherName
                                            })
                                            .Select(grp => new GeneralLedgerMultipleCurrencyReportResult
                                            {
                                                SubjectCode = grp.Key.SubjectCode,
                                                SubjectName = grp.Key.SubjectName,
                                                SubjectOtherName = grp.Key.SubjectOtherName,
                                                NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                                                SortOrder = AccountingCommonConsts.CurrentYearBfOrder,
                                                VoucherCode = AccountingCommonConsts.SystemCodeText,
                                                DocNo = string.Empty,
                                                Description = AccountingCommonConsts.CurrentYearBfText,
                                                VoucherDate = startDate,
                                                CurrencyCode = grp.Key.CurrencyCode,
                                                ForeignAmount = grp.Sum(item => item.ForeignAmount * (int)item.DebitorCreditor),
                                            });

            var currentQuery = _voucherQueryable.Where(item => item.VoucherDate >= startDate && item.VoucherDate <= endDate)
                                            .SelectMany(item => item.Details)
                                            .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                                            .Select(item => new GeneralLedgerMultipleCurrencyReportResult
                                            {
                                                SubjectCode = item.Subject.Code,
                                                SubjectName = item.Subject.Name,
                                                SubjectOtherName = item.Subject.OtherName,
                                                NativeAmount = item.NativeAmount * (int)item.DebitorCreditor,
                                                SortOrder = AccountingCommonConsts.CurrentPeriodOrder,
                                                VoucherCode = item.Voucher.Code,
                                                DocNo = item.DocNo,
                                                Description = item.Description,
                                                VoucherDate = item.Voucher.VoucherDate,
                                                CurrencyCode = item.CurrencyCode,
                                                ForeignAmount = item.ForeignAmount,
                                            });
            var finalQuery = lastYearBfQuery
                                .Concat(currentYearBfQuery)
                                .Concat(currentQuery)
                                .OrderBy(item => item.SubjectCode)
                                .OrderBy(item => item.CurrencyCode)
                                .ThenBy(item => item.SortOrder)
                                .ThenBy(item => item.VoucherDate)
                                .ThenBy(item => item.VoucherCode);

            return await finalQuery.ToListAsync(cancellationToken);
        }
    }
}
