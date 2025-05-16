using Xunit;

namespace Accounting.EntityFrameworkCore;

[CollectionDefinition(AccountingTestConsts.CollectionDefinitionName)]
public class AccountingEntityFrameworkCoreCollection : ICollectionFixture<AccountingEntityFrameworkCoreFixture>
{

}
