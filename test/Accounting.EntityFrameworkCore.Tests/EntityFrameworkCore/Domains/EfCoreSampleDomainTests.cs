using Accounting.Samples;
using Xunit;

namespace Accounting.EntityFrameworkCore.Domains;

[Collection(AccountingTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<AccountingEntityFrameworkCoreTestModule>
{

}
