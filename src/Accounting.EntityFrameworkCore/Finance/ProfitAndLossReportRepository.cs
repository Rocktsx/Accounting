using Accounting.EntityFrameworkCore;
using Accounting.Finance.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.Finance
{
    public class ProfitAndLossReportRepository : VoucherRepository, IProfitAndLossReportRepository
    {
        /// <summary>
        /// Income, Expenses group
        /// </summary>
        private readonly AccountTypeGroup[] _ieGroups = [AccountTypeGroup.Income,
            AccountTypeGroup.Expenses];

        public ProfitAndLossReportRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public async Task<IEnumerable<ProfitAndLossMonthToDateYearToDateResult>> GetMonthToDateAndYearToDateListAsync(
            DateOnly startDate, DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync();
            var plQueryable = queryable.Where(item =>
                item.VoucherDate <= endDate && item.VoucherDate >= periodStartDate)
                .SelectMany(item => item.Details)
               .Where(item => _ieGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => new
               {
                   SubjectCode = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Code : item.Subject.SubjectCategory.Code,
                   SubjectName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Name : item.Subject.SubjectCategory.Name,
                   SubjectOtherName = item.Subject.SubjectCategory.ShowDetail ?
                        item.Subject.OtherName : item.Subject.SubjectCategory.OtherName,
                   item.Subject.AccountTypeId,
                   SortOrder = item.Subject.AccountType.ProfitAndLossSort,
                   Group = item.Subject.AccountType.ProfitAndLossGroup,
                   Category = item.Subject.AccountType.TrialBalanceGroup
               })
              .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0
                    || grp.Sum(item => item.NativeAmount * (item.Voucher.VoucherDate < startDate ? 0 : (int)item.DebitorCreditor)) != 0)
              .Select(grp => new ProfitAndLossMonthToDateYearToDateResult
              {
                  Group = grp.Key.Group,
                  SortOrder = grp.Key.SortOrder,
                  SubjectCode = grp.Key.SubjectCode,
                  SubjectName = grp.Key.SubjectName,
                  SubjectOtherName = grp.Key.SubjectOtherName,
                  Category = grp.Key.Category,
                  AccountTypeId = grp.Key.AccountTypeId,
                  NativeAmount = -grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor), //Income creditor, Expenses debitor
                  MonthToDateNativeAmount = -grp.Sum(item => item.NativeAmount *
                        (item.Voucher.VoucherDate < startDate ? 0 : (int)item.DebitorCreditor)),
                  LastPeriodNativeAmount = -grp.Sum(item => item.NativeAmount *
                        (item.Voucher.VoucherDate >= startDate ? 0 : (int)item.DebitorCreditor)),
              })
              .OrderBy(item => item.Group)
              .ThenBy(item => item.SortOrder)
              .ThenBy(item => item.SubjectCode);

            return await plQueryable.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProfitAndLossYearToDateResult>> GetYearToDateListAsync(
            DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync();
            var plQueryable = queryable.Where(item =>
                item.VoucherDate <= endDate && item.VoucherDate >= periodStartDate)
                .SelectMany(item => item.Details)
               .Where(item => _ieGroups.Contains(item.Subject.AccountType.TrialBalanceGroup))
               .GroupBy(item => new
               {
                   SubjectCode = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Code : item.Subject.SubjectCategory.Code,
                   SubjectName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.Name : item.Subject.SubjectCategory.Name,
                   SubjectOtherName = item.Subject.SubjectCategory.ShowDetail ? item.Subject.OtherName : item.Subject.SubjectCategory.OtherName,
                   item.Subject.AccountTypeId,
                   SortOrder = item.Subject.AccountType.ProfitAndLossSort,
                   Group = item.Subject.AccountType.ProfitAndLossGroup,
                   Category = item.Subject.AccountType.TrialBalanceGroup
               })
              .Where(grp => grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) != 0)
              .Select(grp => new ProfitAndLossYearToDateResult
              {
                  Group = grp.Key.Group,
                  SortOrder = grp.Key.SortOrder,
                  SubjectCode = grp.Key.SubjectCode,
                  SubjectName = grp.Key.SubjectName,
                  SubjectOtherName = grp.Key.SubjectOtherName,
                  Category = grp.Key.Category,
                  AccountTypeId = grp.Key.AccountTypeId,
                  NativeAmount = -grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) //Income creditor, Expenses debitor
              })
              .OrderBy(item => item.Group)
              .ThenBy(item => item.SortOrder)
              .ThenBy(item => item.SubjectCode);

            return await plQueryable.ToListAsync(cancellationToken);
        }
    }
}
