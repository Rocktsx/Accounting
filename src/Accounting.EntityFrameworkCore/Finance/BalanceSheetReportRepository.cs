using Accounting.EntityFrameworkCore;
using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
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
    public class BalanceSheetReportRepository : VoucherRepository, IBalanceSheetReportRepository
    {

        public BalanceSheetReportRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<IEnumerable<BalanceSheetYearToDateResult>> GetYearToDateListAsync(DateOnly endDate,
            DateOnly periodStartDate, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync();

            var capitalAccountTypeId = await GetCapitalAccountTypeIdAsync(cancellationToken);

            var ytdQueryable = queryable.Where(item => item.VoucherDate <= endDate)
                .SelectMany(item => item.Details)
               .Where(item => ALCGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => new
               {
                   SubjectCode = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Code : item.Subject.SubjectCategory.Code,
                   SubjectName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Name : item.Subject.SubjectCategory.Name,
                   SubjectOtherName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.OtherName : item.Subject.SubjectCategory.OtherName,
                   AccountTypeId = item.Subject.SubjectCategory.ShowDetail ? item.Subject.AccountTypeId : item.Subject.SubjectCategory.AccountTypeId,
                   SortOrder = item.Subject.SubjectCategory.ShowDetail ? item.Subject.AccountType.BalanceSheetSort : item.Subject.SubjectCategory.AccountType.BalanceSheetSort,
                   Group = item.Subject.SubjectCategory.ShowDetail ? item.Subject.AccountType.BalanceSheetGroup : item.Subject.SubjectCategory.AccountType.BalanceSheetGroup,
                   AccountTypeGroup = item.Subject.SubjectCategory.ShowDetail ? item.Subject.AccountType.TrialBalanceGroup : item.Subject.SubjectCategory.AccountType.TrialBalanceGroup,
               })
              .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
              .Select(grp => new BalanceSheetYearToDateResult
              {
                  SortOrder = grp.Key.SortOrder,
                  Group = (int)grp.Key.Group,
                  AccountTypeId = grp.Key.AccountTypeId,
                  SubjectCode = grp.Key.SubjectCode,
                  SubjectName = grp.Key.SubjectName,
                  SubjectOtherName = grp.Key.SubjectOtherName,
                  NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                  AccountTypeGroup = grp.Key.AccountTypeGroup
              });

            var mtdIEQueryable = GetYearToDateGroupQueryable(queryable.Where(item => item.VoucherDate <= endDate && item.VoucherDate >= periodStartDate),
                     AccountingCommonConsts.SystemGenGroupSort2, capitalAccountTypeId);

            var lastPeriodIEQueryable = GetYearToDateGroupQueryable(queryable.Where(item => item.VoucherDate < periodStartDate),
                    AccountingCommonConsts.SystemGenGroupSort, capitalAccountTypeId);

            var finalQueryable = ytdQueryable
                .Concat(mtdIEQueryable)
                .Concat(lastPeriodIEQueryable)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.SubjectCode);

            return await finalQueryable.ToListAsync(cancellationToken);
        }
        private IQueryable<BalanceSheetYearToDateResult> GetYearToDateGroupQueryable(IQueryable<Voucher> queryable, int sortOrder, Guid? capitalAccountTypeId)
        {
            return queryable
               .SelectMany(item => item.Details)
               .Where(item => IEGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => 1)
               .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
               .Select(grp => new BalanceSheetYearToDateResult
               {
                   SortOrder = sortOrder,
                   Group = sortOrder,
                   AccountTypeId = capitalAccountTypeId,
                   AccountTypeGroup = AccountTypeGroup.Capital,
                   SubjectCode = AccountingCommonConsts.SystemGenCodeText,
                   SubjectName = AccountingCommonConsts.SubjectName,
                   SubjectOtherName = AccountingCommonConsts.SubjectOtherName,
                   NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor)
               });
        }

        public async Task<IEnumerable<BalanceSheetMonthToDateYearToDateResult>> GetMonthToDateAndYearToDateListAsync(DateOnly startDate,
            DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync();

            var capitalAccountTypeId = await GetCapitalAccountTypeIdAsync(cancellationToken);

            var ytdQueryable = queryable.Where(item => item.VoucherDate <= endDate)
                .SelectMany(item => item.Details)
                .Where(item => ALCGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
                .GroupBy(item => new
                {
                    SubjectCode = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Code : item.Subject.SubjectCategory.Code,
                    SubjectName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Name : item.Subject.SubjectCategory.Name,
                    SubjectOtherName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.OtherName : item.Subject.SubjectCategory.OtherName,
                    AccountTypeId = item.Subject.SubjectCategory.ShowDetail ? item.Subject.AccountTypeId : item.Subject.SubjectCategory.AccountTypeId,
                    SortOrder = item.Subject.SubjectCategory.ShowDetail ? item.Subject.AccountType.BalanceSheetSort : item.Subject.SubjectCategory.AccountType.BalanceSheetSort,
                    Group = item.Subject.SubjectCategory.ShowDetail ? item.Subject.AccountType.BalanceSheetGroup : item.Subject.SubjectCategory.AccountType.BalanceSheetGroup,
                    AccountTypeGroup = item.Subject.SubjectCategory.ShowDetail ? item.Subject.AccountType.TrialBalanceGroup : item.Subject.SubjectCategory.AccountType.TrialBalanceGroup,
                })
                .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0
                            || grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate < periodStartDate ? 0 : (int)item.DebitorCreditor)) != 0)
               .Select(grp => new BalanceSheetMonthToDateYearToDateResult
               {
                   SortOrder = grp.Key.SortOrder,
                   Group = (int)grp.Key.Group,
                   AccountTypeId = grp.Key.AccountTypeId,
                   SubjectCode = grp.Key.SubjectCode,
                   SubjectName = grp.Key.SubjectName,
                   SubjectOtherName = grp.Key.SubjectOtherName,
                   AccountTypeGroup = grp.Key.AccountTypeGroup,
                   NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                   MonthToDateNativeAmount = grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate < startDate ? 0 : (int)item.DebitorCreditor)),
                   LastPeriodNativeAmount = grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate >= periodStartDate ? 0 : (int)item.DebitorCreditor)),
               });

            var mtdIEQueryable = queryable.Where(item => item.VoucherDate <= endDate && item.VoucherDate >= periodStartDate)
                 .SelectMany(item => item.Details)
                 .Where(item => IEGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
                 .GroupBy(item => 1)
                 .Select(grp => new BalanceSheetMonthToDateYearToDateResult
                 {
                     SortOrder = AccountingCommonConsts.SystemGenGroupSort2,
                     Group = AccountingCommonConsts.SystemGenGroupSort2,
                     AccountTypeId = capitalAccountTypeId,
                     SubjectCode = AccountingCommonConsts.SystemGenCodeText,
                     SubjectName = AccountingCommonConsts.SubjectName,
                     SubjectOtherName = AccountingCommonConsts.SubjectOtherName,
                     AccountTypeGroup = AccountTypeGroup.Capital,
                     NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                     MonthToDateNativeAmount = grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate < startDate ? 0 : (int)item.DebitorCreditor)),
                     LastPeriodNativeAmount = grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate >= startDate ? 0 : (int)item.DebitorCreditor)),
                 });

            var lastPeriodIEQueryable = queryable.Where(item => item.VoucherDate < periodStartDate)
               .SelectMany(item => item.Details)
               .Where(item => IEGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => 1)
              .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
              .Select(grp => new BalanceSheetMonthToDateYearToDateResult
              {
                  SortOrder = AccountingCommonConsts.SystemGenGroupSort,
                  Group = AccountingCommonConsts.SystemGenGroupSort,
                  AccountTypeId = capitalAccountTypeId,
                  SubjectCode = AccountingCommonConsts.SystemGenCodeText,
                  SubjectName = AccountingCommonConsts.SubjectName,
                  SubjectOtherName = AccountingCommonConsts.SubjectOtherName,
                  AccountTypeGroup = AccountTypeGroup.Capital,
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
    }
}
