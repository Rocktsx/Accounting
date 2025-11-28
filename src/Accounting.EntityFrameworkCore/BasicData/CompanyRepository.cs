using Accounting.BasicData.Companies;
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
    public class CompanyRepository : EfCoreRepository<AccountingDbContext, Company, Guid>, ICompanyRepository
    {
        public CompanyRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(string? filter = null, bool? isClient = null, bool? isVendor = null, Guid[]? Ids = null, IEnumerable<string>? codes = null, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryable(filter, isClient, isVendor, Ids, codes))
              .LongCountAsync(cancellationToken);
        }
        private async Task<IQueryable<Company>> GetQueryable(string? filter = null, bool? isClient = null, bool? isVendor = null, Guid[]? ids = null, IEnumerable<string>? codes = null)
        {
            return (await GetDbSetAsync())
             .WhereIf(!string.IsNullOrWhiteSpace(filter), item => item.Name.Contains(filter)
                || item.Code.Contains(filter) || item.OtherName.Contains(filter) || item.NickName.Contains(filter))
             .WhereIf(isClient.HasValue && isClient == true, item => item.IsClient == true)
             .WhereIf(isVendor.HasValue && isVendor == true, item => item.IsVendor == true)
             .WhereIf(ids != null && ids.Length > 0, item => ids.Contains(item.Id))
             .WhereIf(codes != null && codes.Count() > 0, item => codes.Contains(item.Code));
        }
        public async Task<long> GetLastNumber(string prefix, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .Where(item => item.Prefix == prefix)
                .OrderByDescending(item => item.GenNo)
                .Select(item => item.GenNo)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Company>> GetPagedListAsync(string filter = null, bool? isClient = null, bool? isVendor = null, Guid[]? Ids = null, IEnumerable<string>? codes = null, bool includeDetails = false, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryable(filter, isClient, isVendor, Ids, codes))
              .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(Company.Id) : sorting)
              .Skip(skipCount).Take(maxResultCount)
              .ToListAsync(cancellationToken);
        }
        public async Task<Company?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
                return await (await GetDbSetAsync())
                .Include(item => item.Addresses)
                .Include(item => item.Contacts) 
                .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        }
    }
}
