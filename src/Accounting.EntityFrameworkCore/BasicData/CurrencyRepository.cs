using Accounting.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.BasicData
{
    public class CurrencyRepository : EfCoreRepository<AccountingDbContext, Currency, Guid>, ICurrencyRepository
    {
        public CurrencyRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
