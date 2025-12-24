using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.BankReconciliations
{
    public interface IBankReconciliationRepository : IBasicRepository<BankReconciliation, Guid>
    {
        Task<IEnumerable<BankReconciliationPagedResult>> GetPagedListAsync(
           BankReconciliationFilterRequest request = null,
           string sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(BankReconciliationFilterRequest request = null,
            bool showVouchers = true, CancellationToken cancellationToken = default);
    }
}
