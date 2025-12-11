using Accounting.BasicData;
using Xunit;

namespace Accounting.EntityFrameworkCore.Domains
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreCompanyRepositoryTests : CompanyRepositoryTests<AccountingEntityFrameworkCoreTestModule>
    { 
    }
}
