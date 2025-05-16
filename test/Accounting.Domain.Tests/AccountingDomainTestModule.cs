using Volo.Abp.Modularity;

namespace Accounting;

[DependsOn(
    typeof(AccountingDomainModule),
    typeof(AccountingTestBaseModule)
)]
public class AccountingDomainTestModule : AbpModule
{

}
