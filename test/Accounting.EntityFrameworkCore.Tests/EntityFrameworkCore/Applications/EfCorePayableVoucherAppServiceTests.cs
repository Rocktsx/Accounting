using Accounting.Finance;
using Xunit;

namespace Accounting.EntityFrameworkCore.Applications
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCorePayableVoucherAppServiceTests: PayableVoucherAppServiceTests<AccountingEntityFrameworkCoreTestModule>
    {
    }
}
