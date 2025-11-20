using Accounting.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Account;
using Volo.Abp.Mapperly;
using Volo.Abp.DistributedLocking;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Accounting;

[DependsOn(
    typeof(AbpDistributedLockingAbstractionsModule),
    typeof(AccountingDomainModule),
    typeof(AccountingApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class AccountingApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    { 
        context.Services.AddTransient<CodeGenerator>();
    }
}
