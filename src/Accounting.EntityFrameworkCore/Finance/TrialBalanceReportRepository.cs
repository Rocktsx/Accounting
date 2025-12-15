using Accounting.EntityFrameworkCore;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.Finance
{
    public class TrialBalanceReportRepository : VoucherRepository, ITrialBalanceReportRepository
    {
        public TrialBalanceReportRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        
        public async Task<IEnumerable<TrialBalanceYearToDateResult>> GetYearToDateListAsync(DateOnly endDate,
            DateOnly periodStartDate, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync();

            var capitalAccountTypeId = await GetCapitalAccountTypeIdAsync(cancellationToken);

            var ytdQueryable = GetYearToDateGroupQueryable(queryable.Where(item => item.VoucherDate <= endDate), ALCGroups);

            var mtdIEQueryable = GetYearToDateGroupQueryable(queryable.Where(item => 
                item.VoucherDate <= endDate && item.VoucherDate >= periodStartDate), IEGroups);

            var lastPeriodIEQueryable = queryable.Where(item => item.VoucherDate < periodStartDate)
               .SelectMany(item => item.Details)
               .Where(item => IEGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => 1)
              .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
              .Select(grp => new TrialBalanceYearToDateResult
              {
                  SortOrder = AccountingCommonConsts.SystemGenGroupSort,
                  Group = AccountingCommonConsts.SystemGenGroupSort,
                  AccountTypeId = capitalAccountTypeId,
                  SubjectCode = AccountingCommonConsts.SystemGenCodeText,
                  SubjectName = AccountingCommonConsts.SubjectName,
                  SubjectOtherName = AccountingCommonConsts.SubjectOtherName,
                  NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor)
              });

            var finalQueryable = ytdQueryable
                .Concat(mtdIEQueryable)
                .Concat(lastPeriodIEQueryable)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.SubjectCode);

            return await finalQueryable.ToListAsync(cancellationToken);
        }
        private IQueryable<TrialBalanceYearToDateResult> GetYearToDateGroupQueryable(IQueryable<Voucher> queryable, AccountTypeGroup[] groups)
        {
            return queryable
               .SelectMany(item => item.Details)
               .Where(item => groups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => new
               {
                   SubjectCode = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Code : item.Subject.SubjectCategory.Code,
                   SubjectName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Name : item.Subject.SubjectCategory.Name,
                   SubjectOtherName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.OtherName : item.Subject.SubjectCategory.OtherName,
                   item.Subject.AccountTypeId,
                   SortOrder = item.Subject.AccountType.TrialBalanceSort,
                   Group = item.Subject.AccountType.TrialBalanceGroup
               })
              .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
              .Select(grp => new TrialBalanceYearToDateResult
              {
                  SortOrder = grp.Key.SortOrder,
                  Group = (int)grp.Key.Group,
                  AccountTypeId = grp.Key.AccountTypeId,
                  SubjectCode = grp.Key.SubjectCode,
                  SubjectName = grp.Key.SubjectName,
                  SubjectOtherName = grp.Key.SubjectOtherName,
                  NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor)
              });
        }
        
        public async Task<IEnumerable<TrialBalanceMonthToDateYearToDateResult>> GetMonthToDateAndYearToDateListAsync(DateOnly startDate,
            DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync();

            var capitalAccountTypeId = await GetCapitalAccountTypeIdAsync(cancellationToken);

            var ytdQueryable = GetMonthToDateAndYearToDateGroupQueryable(queryable.Where(item => 
                item.VoucherDate <= endDate), ALCGroups, startDate, periodStartDate); 

            var mtdIEQueryable = GetMonthToDateAndYearToDateGroupQueryable(queryable.Where(item =>
                item.VoucherDate <= endDate && item.VoucherDate >= periodStartDate), IEGroups, startDate, startDate);
            

            var lastPeriodIEQueryable = queryable.Where(item => item.VoucherDate < periodStartDate)
               .SelectMany(item => item.Details)
               .Where(item => IEGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => 1)
              .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
              .Select(grp => new TrialBalanceMonthToDateYearToDateResult
              {
                  SortOrder = AccountingCommonConsts.SystemGenGroupSort,
                  Group = AccountingCommonConsts.SystemGenGroupSort,
                  AccountTypeId = capitalAccountTypeId,
                  SubjectCode = AccountingCommonConsts.SystemGenCodeText,
                  SubjectName = AccountingCommonConsts.SubjectName,
                  SubjectOtherName = AccountingCommonConsts.SubjectOtherName,
                  NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                  MonthToDateNativeAmount = 0,
                  LastPeriodNativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
              });

            var finalQueryable = ytdQueryable
                .Concat(mtdIEQueryable)
                .Concat(lastPeriodIEQueryable)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.SubjectCode);

            return await finalQueryable.ToListAsync(cancellationToken);
        }
        private IQueryable<TrialBalanceMonthToDateYearToDateResult> GetMonthToDateAndYearToDateGroupQueryable(
            IQueryable<Voucher> queryable, AccountTypeGroup[] groups, DateOnly startDate,DateOnly endDate)
        {
            return queryable
               .SelectMany(item => item.Details)
               .Where(item => groups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => new
               {
                   SubjectCode = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Code : item.Subject.SubjectCategory.Code,
                   SubjectName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Name : item.Subject.SubjectCategory.Name,
                   SubjectOtherName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.OtherName : item.Subject.SubjectCategory.OtherName,
                   item.Subject.AccountTypeId,
                   SortOrder = item.Subject.AccountType.TrialBalanceSort,
                   Group = item.Subject.AccountType.TrialBalanceGroup
               })
               .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0
                            || grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate < endDate ? 0 : (int)item.DebitorCreditor)) != 0)
              .Select(grp => new TrialBalanceMonthToDateYearToDateResult
              {
                  SortOrder = grp.Key.SortOrder,
                  Group = (int)grp.Key.Group,
                  AccountTypeId = grp.Key.AccountTypeId,
                  SubjectCode = grp.Key.SubjectCode,
                  SubjectName = grp.Key.SubjectName,
                  SubjectOtherName = grp.Key.SubjectOtherName,
                  NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                  MonthToDateNativeAmount = grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate < startDate ? 0 : (int)item.DebitorCreditor)),
                  LastPeriodNativeAmount = grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate >= endDate ? 0 : (int)item.DebitorCreditor)),
              });
        }
    }
}
