using Accounting.EntityFrameworkCore;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
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
    public class SubjectRepository : EfCoreRepository<AccountingDbContext, Subject, Guid>, ISubjectRepository
    {
        public SubjectRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(SubjectFilterRequest request = null, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryable(request))
                .LongCountAsync(cancellationToken);
        }

        public async Task<IEnumerable<Subject>> GetPagedListAsync(SubjectFilterRequest request = null, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryable(request))
              .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(SubjectCategory.Id) : sorting)
              .Skip(skipCount).Take(maxResultCount)
              .ToListAsync(cancellationToken);
        }
        private async Task<IQueryable<Subject>> GetQueryable(SubjectFilterRequest request)
        {
            var queryable = (await GetDbSetAsync()).AsQueryable();
            if (request == null)
            {
                return queryable;
            }
            if (request.IsIncludeAccountType == true)
            {
                queryable = queryable.Include(item => item.AccountType);
            }
            return queryable
             .WhereIf(!string.IsNullOrWhiteSpace(request.Filter), item => item.Code.Contains(request.Filter)
                    || item.Name.Contains(request.Filter) || item.OtherName.Contains(request.Filter))
             .WhereIf(request.SubjectCategoryId != null, item => item.SubjectCategoryId == request.SubjectCategoryId)
             .WhereIf(request.SubjectIds != null && request.SubjectIds.Count() > 0, item => request.SubjectIds.Contains(item.Id))
             .WhereIf(request.Codes != null && request.Codes.Count() > 0, item => request.Codes.Contains(item.Code))
             .WhereIf(request.IsPaymentMethod != null, item => item.IsPayMethod == request.IsPaymentMethod)
             .WhereIf(request.AccountTypeCategory != null, item => item.AccountType.Category == request.AccountTypeCategory);
        }
    }
}
