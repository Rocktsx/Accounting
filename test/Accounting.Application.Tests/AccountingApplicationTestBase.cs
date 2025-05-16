using Volo.Abp.Modularity;

namespace Accounting;

public abstract class AccountingApplicationTestBase<TStartupModule> : AccountingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
