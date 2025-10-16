using Accounting.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.Finance
{
    public class AccountTypeRepository: EfCoreRepository<AccountingDbContext, AccountType, Guid>, IAccountTypeRepository
    {
        public AccountTypeRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    } 
}
