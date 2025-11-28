using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.AccountingPeriods
{
    public interface IAccountingPeriodRepository : IBasicRepository<AccountingPeriod, Guid>
    {
        Task<IEnumerable<AccountingPeriod>> GetPagedListAsync(
            string filter = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<AccountingPeriod>> GetCurrentPeriodsAsync(CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default); 
    }
}
