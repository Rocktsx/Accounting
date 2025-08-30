using System.Threading.Tasks;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;

namespace Accounting.Web.Menus;

public class AccountingMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<AccountingResource>();

        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                AccountingMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );


        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 7);

        var basicDataMenu = new ApplicationMenuItem(
             AccountingPermissions.BasicDataGroupName,
             l["Menu:BasicData"],
             icon: "fas fa-gears"
         );
        if (await context.IsGrantedAsync(AccountingPermissions.Currency))
        {
            basicDataMenu.AddItem(
              new ApplicationMenuItem(
              AccountingPermissions.Currency,
              l["Menu:Currency"],
                icon: "fas fa-dollar-sign",
              url: "/BasicData/Currency"
              )
          );
        }

        if (await context.IsGrantedAsync(AccountingPermissions.Client))
        {
            basicDataMenu.AddItem(
             new ApplicationMenuItem(
                 AccountingPermissions.Client,
                 l["Menu:Client"],
                   icon: "fas fa-credit-card",
                 url: "/BasicData/Client"
                 )
            );
        }

        if (await context.IsGrantedAsync(AccountingPermissions.Vendor))
        {
            basicDataMenu.AddItem(
                new ApplicationMenuItem(
                AccountingPermissions.Vendor,
                l["Menu:Vendor"],
                  icon: "fas fa-rectangle-list",
                url: "/BasicData/Vendor"
                )
            );
        }
        if (basicDataMenu.Items.Count > 0)
        {
            context.Menu.AddItem(basicDataMenu);
        }
        var genenalLedgerMenu = new ApplicationMenuItem(
           AccountingPermissions.GenenalLedgerGroupName,
           l["Menu:GenenalLedger"],
           icon: "fas fa-calculator"
       );
        if (await context.IsGrantedAsync(AccountingPermissions.AccountingPeriod))
        {
            genenalLedgerMenu.AddItem(
                new ApplicationMenuItem(
                AccountingPermissions.AccountingPeriod,
                l["Menu:AccountingPeriod"],
                  icon: "fas fa-bars-staggered",
                url: "/GenenalLedger/AccountingPeriod"
                )
            );
        }
        if (genenalLedgerMenu.Items.Count > 0)
        {
            context.Menu.AddItem(genenalLedgerMenu);
        }
    }
}
