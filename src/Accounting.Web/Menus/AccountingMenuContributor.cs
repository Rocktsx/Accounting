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

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
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
             AccountingMenus.BasicData,
             l[AccountingMenus.DisplayNames.BasicData],
             icon: "fas fa-gears"
         );
        basicDataMenu.AddItem(
              new ApplicationMenuItem(
                  AccountingMenus.Currency,
                  l[AccountingMenus.DisplayNames.Currency],
                  icon: "fas fa-dollar-sign",
                  url: "/BasicData/Currencies"
              ).RequirePermissions(AccountingPermissions.Currencies.Default)
          );

        basicDataMenu.AddItem(
            new ApplicationMenuItem(
                    AccountingMenus.Client,
                    l[AccountingMenus.DisplayNames.Client],
                    icon: "fas fa-credit-card",
                    url: "/BasicData/Clients"
                ).RequirePermissions(AccountingPermissions.Clients.Default)
        );

        basicDataMenu.AddItem(
            new ApplicationMenuItem(
                AccountingMenus.Vendor,
                l[AccountingMenus.DisplayNames.Vendor],
                icon: "fas fa-rectangle-list",
                url: "/BasicData/Vendors"
            ).RequirePermissions(AccountingPermissions.Vendors.Default)
        );

        if (basicDataMenu.Items.Count > 0)
        {
            context.Menu.AddItem(basicDataMenu);
        }
        var generalLedgerMenu = new ApplicationMenuItem(
           AccountingMenus.GeneralLedger,
           l[AccountingMenus.DisplayNames.GeneralLedger],
           icon: "fas fa-calculator"
        );

        generalLedgerMenu.AddItem(
            new ApplicationMenuItem(
                AccountingMenus.AccountingPeriod,
                l[AccountingMenus.DisplayNames.AccountingPeriod],
                icon: "fas fa-bars-staggered",
                url: "/GeneralLedger/AccountingPeriods"
            ).RequirePermissions(AccountingPermissions.AccountingPeriods.Default)
        );

        generalLedgerMenu.AddItem(
            new ApplicationMenuItem(
                AccountingMenus.SubjectCategory,
                l[AccountingMenus.DisplayNames.SubjectCategory],
                icon: "fas fa-landmark",
                url: "/GeneralLedger/SubjectCategories"
            ).RequirePermissions(AccountingPermissions.SubjectCategories.Default)
        );

        generalLedgerMenu.AddItem(
            new ApplicationMenuItem(
                AccountingMenus.GeneralAccount,
                l[AccountingMenus.DisplayNames.GeneralAccount],
                icon: "fas fa-bug",
                url: "/GeneralLedger/GeneralAccounts"
            ).RequirePermissions(AccountingPermissions.GeneralAccounts.Default)
        );

        generalLedgerMenu.AddItem(
            new ApplicationMenuItem(
                AccountingMenus.Subject,
                l[AccountingMenus.DisplayNames.Subject],
                icon: "fas fa-key",
                url: "/GeneralLedger/ChartOfAccount"
            ).RequirePermissions(AccountingPermissions.Subjects.Default)
        );

        generalLedgerMenu.AddItem(
               new ApplicationMenuItem(
                   AccountingMenus.TransferVoucher,
                   l[AccountingMenus.DisplayNames.TransferVoucher],
                   icon: "fas fa-wand-magic-sparkles",
                   url: "/GeneralLedger/TransferVouchers"
               ).RequirePermissions(AccountingPermissions.TransferVouchers.Default)
         );
        generalLedgerMenu.AddItem(
               new ApplicationMenuItem(
                   AccountingMenus.VoucherState,
                   l[AccountingMenus.DisplayNames.VoucherState],
                   icon: "fas fa-layer-group",
                   url: "/GeneralLedger/VoucherStates"
               ).RequirePermissions(AccountingPermissions.VoucherStates.Default)
         );
        if (generalLedgerMenu.Items.Count > 0)
        {
            context.Menu.AddItem(generalLedgerMenu);
        }

        return Task.CompletedTask;
    }
}
