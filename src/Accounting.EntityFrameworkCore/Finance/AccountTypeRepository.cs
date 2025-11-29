using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;
using Accounting.EntityFrameworkCore;
using Accounting.Finance.AccountTypes;
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
    public class AccountTypeRepository: EfCoreRepository<AccountingDbContext, AccountType, Guid>, IAccountTypeRepository
    {
        public AccountTypeRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(string? filter = null, IEnumerable<Guid?> ids = null, IEnumerable<string> codes = null, CancellationToken cancellationToken = default)
        {
            return await(await GetQueryable(filter, ids, codes))  
               .LongCountAsync(cancellationToken);
        }

        public async Task<IEnumerable<AccountType>> GetPagedListAsync(string filter = null, IEnumerable<Guid?> ids = null, IEnumerable<string> codes = null, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            return await(await GetQueryable(filter,ids, codes))
              .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(AccountType.Id) : sorting)
              .Skip(skipCount).Take(maxResultCount)
              .ToListAsync(cancellationToken);
        }
        private async Task<IQueryable<AccountType>> GetQueryable(string? filter = null, IEnumerable<Guid?> ids = null, IEnumerable<string>? codes = null)
        {
            return (await GetDbSetAsync())
              .WhereIf(!string.IsNullOrWhiteSpace(filter), item =>item.Code.Contains(filter) || item.Name.Contains(filter)
                     || item.OtherName.Contains(filter))
             .WhereIf(ids != null && ids.Count() > 0, item => ids.Contains(item.Id))
             .WhereIf(codes != null && codes.Count() > 0, item => codes.Contains(item.Code));
        }
    } 
}
