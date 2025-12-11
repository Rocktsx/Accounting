using Accounting.BasicData;
using Xunit;

namespace Accounting.EntityFrameworkCore.Domains
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreCurrencyRepositoryTests : CurrencyRepositoryTests<AccountingEntityFrameworkCoreTestModule>
    { 
    }
}
