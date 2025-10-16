using Accounting.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.BasicData
{
    public class CompanyRepository : EfCoreRepository<AccountingDbContext, Company, Guid>, ICompanyRepository
    {
        public CompanyRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
