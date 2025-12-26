using Accounting.Finance;
using Xunit;

namespace Accounting.EntityFrameworkCore.Applications
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreBankReconciliationReportAppServiceTests:
        BankReconciliationReportAppServiceTests<AccountingEntityFrameworkCoreTestModule>
    {
    }
}
