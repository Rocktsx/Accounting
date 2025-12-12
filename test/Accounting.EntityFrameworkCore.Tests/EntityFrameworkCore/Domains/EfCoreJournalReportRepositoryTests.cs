using Accounting.Finance;
using Xunit;

namespace Accounting.EntityFrameworkCore.Domains
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreJournalReportRepositoryTests :
        JournalReportRepositoryTests<AccountingEntityFrameworkCoreTestModule>
    {
    }
}
