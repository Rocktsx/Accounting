using Accounting.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.Finance
{
    public class SubjectCategoryRepository : EfCoreRepository<AccountingDbContext, SubjectCategory, Guid>, ISubjectCategoryRepository
    {
        public SubjectCategoryRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
