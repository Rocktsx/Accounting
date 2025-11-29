using Accounting.EntityFrameworkCore;
using Accounting.Finance.SubjectCategories;
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
    public class SubjectCategoryRepository : EfCoreRepository<AccountingDbContext, SubjectCategory, Guid>, ISubjectCategoryRepository
    {
        public SubjectCategoryRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public async Task<long> GetCountAsync(string? filter = null, IEnumerable<Guid?> ids = null, IEnumerable<string> codes = null, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryable(filter, ids, codes))
               .LongCountAsync(cancellationToken);
        }

        public async Task<IEnumerable<SubjectCategory>> GetPagedListAsync(string filter = null, IEnumerable<Guid?> ids = null, IEnumerable<string> codes = null, bool isIncludeAccountType = false, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryable(filter, ids, codes, isIncludeAccountType))
              .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(SubjectCategory.Id) : sorting)
              .Skip(skipCount).Take(maxResultCount)
              .ToListAsync(cancellationToken);
        }

        private async Task<IQueryable<SubjectCategory>> GetQueryable(string? filter = null, IEnumerable<Guid?> ids = null, IEnumerable<string>? codes = null, bool isIncludeAccountType = false)
        {
            var queryable = (await GetDbSetAsync()).AsQueryable();
            if (isIncludeAccountType)
            {
                queryable = queryable.Include(item => item.AccountType);
            }
            return queryable
             .WhereIf(!string.IsNullOrWhiteSpace(filter), item => item.Code.Contains(filter)
                    || item.Name.Contains(filter) || item.OtherName.Contains(filter))
             .WhereIf(ids != null && ids.Count() > 0, item => ids.Contains(item.Id))
             .WhereIf(codes != null && codes.Count() > 0, item => codes.Contains(item.Code));
        }
    }
}
