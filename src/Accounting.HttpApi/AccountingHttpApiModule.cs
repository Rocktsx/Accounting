using Localization.Resources.AbpUi;
using Accounting.Localization;
using Volo.Abp.Account;
using Volo.Abp.SettingManagement;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.Localization;
using Volo.Abp.TenantManagement;
using Volo.Abp.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Magicodes.ExporterAndImporter.Csv;
using Magicodes.ExporterAndImporter.Excel;
namespace Accounting;

 [DependsOn(
    typeof(AccountingApplicationContractsModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule)
    )]
public class AccountingHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AccountingHttpApiModule>();
        });
        context.Services.AddScoped<ICsvImporter, CsvImporter>();
        context.Services.AddScoped<IExcelImporter,  ExcelImporter>();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AccountingResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
