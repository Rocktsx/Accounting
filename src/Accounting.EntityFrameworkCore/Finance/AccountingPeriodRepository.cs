using Accounting.EntityFrameworkCore;
using Accounting.Finance.AccountingPeriods;
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
    public class AccountingPeriodRepository : EfCoreRepository<AccountingDbContext, AccountingPeriod, Guid>, IAccountingPeriodRepository
    {
        public AccountingPeriodRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<IEnumerable<AccountingPeriod>> GetPagedListAsync(string filter = null, string sorting = null,
            int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(filter), item => item.Code.Contains(filter))
                .OrderBy(sorting)
                .Skip(skipCount).Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<AccountingPeriod>> GetCurrentPeriodsAsync(CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .Where(item => item.IsCurrentPeriod)
                .ToListAsync(cancellationToken);
        }
        public async Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
             .WhereIf(!string.IsNullOrWhiteSpace(filter), item => item.Code.Contains(filter))
             .LongCountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
