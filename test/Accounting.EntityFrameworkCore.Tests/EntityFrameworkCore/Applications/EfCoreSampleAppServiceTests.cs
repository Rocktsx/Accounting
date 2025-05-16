using Accounting.Samples;
using Xunit;

namespace Accounting.EntityFrameworkCore.Applications;

[Collection(AccountingTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<AccountingEntityFrameworkCoreTestModule>
{

}
