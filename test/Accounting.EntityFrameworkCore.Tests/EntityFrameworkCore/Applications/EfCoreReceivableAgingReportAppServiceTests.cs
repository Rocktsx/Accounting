using Accounting.Finance;
using Xunit;

namespace Accounting.EntityFrameworkCore.Applications
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreReceivableAgingReportAppServiceTests:
        ReceivableAgingReportAppServiceTests<AccountingEntityFrameworkCoreTestModule>
    {
    }
}
