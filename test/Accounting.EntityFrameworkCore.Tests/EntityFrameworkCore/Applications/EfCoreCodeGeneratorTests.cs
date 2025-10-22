using Accounting.Common;
using Xunit;

namespace Accounting.EntityFrameworkCore.Applications
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreCodeGeneratorTests: CodeGeneratorTests<AccountingEntityFrameworkCoreTestModule>
    {
    }
}
