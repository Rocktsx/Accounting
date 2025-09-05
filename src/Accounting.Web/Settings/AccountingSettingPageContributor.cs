using System.Threading.Tasks;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Settings;
using Accounting.Web.Components.AccountingSettingGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

namespace Accounting.Web.Settings;

public class AccountingSettingPageContributor : ISettingPageContributor
{
    public Task ConfigureAsync(SettingPageCreationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AccountingResource>>();
        context.Groups.Add(
            new SettingPageGroup(
                AccountingSettings.GroupName,
                l[AccountingSettings.GroupName],
                typeof(AccountingSettingGroupViewComponent),
                order: 1
            )
        );

        return Task.CompletedTask;
    }

    public Task<bool> CheckPermissionsAsync(SettingPageCreationContext context)
    {
        // You can check the permissions here 
       var service = context.ServiceProvider.GetRequiredService<IAuthorizationService>();
       return service.IsGrantedAsync(AccountingPermissions.AccountingSetting); 
    }
}