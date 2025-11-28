using Accounting.BasicData.Currencies;
using Accounting.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.BasicData
{
    public class CurrencyRepository : EfCoreRepository<AccountingDbContext, Currency, Guid>, ICurrencyRepository
    {
        public CurrencyRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(string? filter = null, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
               .WhereIf(!string.IsNullOrWhiteSpace(filter), item => item.TargetCurrency.Contains(filter))
               .WhereIf(isActive != null, item => item.IsActive == isActive)
               .LongCountAsync(cancellationToken);
        }

        public async Task<IEnumerable<Currency>> GetPagedListAsync(string filter = null, bool? isActive = null, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
               .WhereIf(!string.IsNullOrWhiteSpace(filter), item => item.TargetCurrency.Contains(filter))
               .WhereIf(isActive != null, item => item.IsActive == isActive)
               .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(Currency.TargetCurrency) : sorting)
               .Skip(skipCount).Take(maxResultCount)
               .ToListAsync(cancellationToken);
        }
    }
}
