using Accounting.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.Finance
{
    public class AccountingPeriodRepository : EfCoreRepository<AccountingDbContext, AccountingPeriod, Guid>, IAccountingPeriodRepository
    {
        public AccountingPeriodRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
