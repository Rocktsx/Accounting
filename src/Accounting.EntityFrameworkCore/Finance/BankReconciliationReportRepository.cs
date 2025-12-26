using Accounting.EntityFrameworkCore;
using Accounting.Finance.BankReconciliations;
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
    public class BankReconciliationReportRepository : VoucherRepository, IBankReconciliationReportRepository
    {
        public BankReconciliationReportRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) :
            base(dbContextProvider)
        {
        }

        public async Task<IEnumerable<BankReconciliationReportResult>> GetReportListAsync(DateOnly startDate, DateOnly endDate,
            Guid? subjectId = null, CancellationToken cancellationToken = default)
        {
            var queryable = (await GetQueryableWithDetailsAsync());
            var presentedBeforeQueryable = queryable.Where(item => item.VoucherDate < startDate)
                .SelectMany(item => item.Details)
                .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                .Where(item => item.Subject.AccountType.Category == AccountTypeTypes.Bank)
                .Where(item => item.BankReconciliation.IsPresented == true)
                .GroupBy(item => new
                {
                    item.SubjectId,
                    SubjectCode = item.Subject.Code,
                    SubjectName = item.Subject.Name,
                    SubjectOtherName = item.Subject.OtherName,
                })
                .Select(grp => new BankReconciliationReportResult
                {
                    SubjectCode = grp.Key.SubjectCode,
                    SubjectName = grp.Key.SubjectName,
                    SubjectOtherName = grp.Key.SubjectOtherName,
                    SortOrder = 1,
                    VoucherDate = startDate,
                    NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                    Description = AccountingCommonConsts.PresentedBalanceBf,
                    IsPresented = true,
                    PresentedAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                    DebitorCreditor = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) > 0 ?
                        DebitorCreditor.Debitor : DebitorCreditor.Creditor
                });

            var unpresentedBeforeQueryable = queryable.Where(item => item.VoucherDate < startDate)
                .SelectMany(item => item.Details)
                .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                .Where(item => item.BankReconciliation == null ||
                        item.BankReconciliation.IsPresented == false)
                .Where(item => item.Subject.AccountType.Category == AccountTypeTypes.Bank)
                .GroupBy(item => new
                {
                    item.SubjectId,
                    SubjectCode = item.Subject.Code,
                    SubjectName = item.Subject.Name,
                    SubjectOtherName = item.Subject.OtherName,
                })
                .Select(grp => new BankReconciliationReportResult
                {
                    SubjectCode = grp.Key.SubjectCode,
                    SubjectName = grp.Key.SubjectName,
                    SubjectOtherName = grp.Key.SubjectOtherName,
                    SortOrder = 2,
                    VoucherDate = startDate,
                    NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                    Description = AccountingCommonConsts.UnpresentedBalanceBf,
                    IsPresented = false,
                    PresentedAmount = 0,
                    DebitorCreditor = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor) > 0 ?
                        DebitorCreditor.Debitor : DebitorCreditor.Creditor
                });

            var currentQueryable = queryable.Where(item => item.VoucherDate <= endDate && item.VoucherDate >= startDate)
                .SelectMany(item => item.Details)
                .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                .Where(item => item.Subject.AccountType.Category == AccountTypeTypes.Bank)
                .Select(item => new BankReconciliationReportResult
                {
                    SubjectCode = item.Subject.Code,
                    SubjectName = item.Subject.Name,
                    SubjectOtherName = item.Subject.OtherName,
                    SortOrder = 3,
                    VoucherDate = item.Voucher.VoucherDate,
                    NativeAmount = item.NativeAmount,
                    Description = item.Description,
                    IsPresented = item.BankReconciliation == null ? false : item.BankReconciliation.IsPresented,
                    PresentedAmount = item.BankReconciliation.IsPresented ? item.NativeAmount : 0,
                    DebitorCreditor = item.DebitorCreditor
                });

            var resultQueryable = presentedBeforeQueryable
                .Concat(unpresentedBeforeQueryable)
                .Concat(currentQueryable)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.SubjectCode)
                .ThenBy(item => item.VoucherDate);

            return await resultQueryable.ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<BankReconciliationUnpresentedReportResult>> GetUnpresentedReportListAsync(
            DateOnly startDate, DateOnly endDate, Guid? subjectId = null, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableWithDetailsAsync(); 
            var resultQueryable = queryable.Where(item => item.VoucherDate <= endDate && item.VoucherDate >= startDate)
                .SelectMany(item => item.Details)
                .WhereIf(subjectId != null, item => item.SubjectId == subjectId)
                .Where(item => item.Subject.AccountType.Category == AccountTypeTypes.Bank)
                .Where(item => item.BankReconciliation == null ||
                        item.BankReconciliation.IsPresented == false)
                .Select(item => new BankReconciliationUnpresentedReportResult
                {
                    SubjectCode = item.Subject.Code,
                    SubjectName = item.Subject.Name,
                    SubjectOtherName = item.Subject.OtherName,
                    VoucherDate = item.Voucher.VoucherDate,
                    NativeAmount = item.NativeAmount,
                    Description = item.Description,
                    DebitorCreditor = item.DebitorCreditor
                })
                .OrderBy(item => item.SubjectCode)
                .ThenBy(item => item.VoucherDate);

            return await resultQueryable.ToListAsync(cancellationToken);
        }
    }
}
