using Accounting.EntityFrameworkCore;
using Accounting.Finance.BankReconciliations;
using Accounting.Finance.Vouchers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.Finance
{
    public class BankReconciliationRepository : EfCoreRepository<AccountingDbContext, BankReconciliation, Guid>, IBankReconciliationRepository
    {
        public BankReconciliationRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(BankReconciliationFilterRequest request = null,
            bool showVouchers = true, CancellationToken cancellationToken = default)
        {
            if (showVouchers)
            {
                return await (await GetQueryableAsync(request)).LongCountAsync(cancellationToken);
            }
            return await (await GetQueryableAsync()).LongCountAsync(cancellationToken);
        }

        public async Task<IEnumerable<BankReconciliationPagedResult>> GetPagedListAsync(BankReconciliationFilterRequest request = null, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var queryable = (await GetQueryableAsync(request))
              .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(BankReconciliation.Id) : sorting)
              .Skip(skipCount).Take(maxResultCount)
              .Select(item => new BankReconciliationPagedResult
              {
                  Id = item.BankReconciliation == null ? null : item.BankReconciliation.Id,
                  VoucherDetailId = item.Id,
                  IsPresented = item.BankReconciliation != null && item.BankReconciliation.IsPresented,
                  VoucherDate = item.Voucher.VoucherDate,
                  VoucherCode = item.Voucher.Code,
                  PaymentReference = item.PaymentReference,
                  DebitorCreditor = item.DebitorCreditor,
                  NativeAmount = item.NativeAmount,
                  Description = item.Description,
              });
            return await queryable.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BankReconciliation>> GetListAsync(IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync();
            return await queryable.Where(item => ids.Contains(item.Id)).ToListAsync(cancellationToken);
        }
        private async Task<IQueryable<VoucherDetail>> GetQueryableAsync(BankReconciliationFilterRequest request)
        {
            var queryable = (await GetDbContextAsync()).Set<Voucher>().AsQueryable()
                    .Where(new NoVoidVoucherSpecification().ToExpression());
            if (request == null)
            {
                return queryable.SelectMany(item => item.Details);
            }
            return queryable
                .WhereIf(!string.IsNullOrWhiteSpace(request.Prefix),
                    item => item.Prefix.Contains(request.Prefix))
                .WhereIf(request.StartNo != null, item => item.GenNo >= request.StartNo)
                .WhereIf(request.EndNo != null, item => item.GenNo <= request.EndNo)
                .WhereIf(request.StartDate != null, item => item.VoucherDate >= request.StartDate)
                .WhereIf(request.EndDate != null, item => item.VoucherDate <= request.EndDate)
                .SelectMany(item => item.Details)
                .Where(item => item.Subject.AccountType.Category == AccountTypeTypes.Bank)
                .WhereIf(request.SubjectId != null, item => item.SubjectId == request.SubjectId)
                .WhereIf(!string.IsNullOrWhiteSpace(request.ReferenceNo),
                    item => item.PaymentReference.Contains(request.ReferenceNo))
                .WhereIf(request.IsPresented != null,
                    item => request.IsPresented == false ?
                    item.BankReconciliation == null || item.BankReconciliation.IsPresented == false
                    : item.BankReconciliation.IsPresented == true);
        }
    }
}
