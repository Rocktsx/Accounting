using System.Threading.Tasks;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.Features;
using Accounting.Features;

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
            ).RequireFeatures(AccountingFeatures.AccountTypeFunction)
            .RequirePermissions(AccountingPermissions.SubjectCategories.Default)
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
                url: "/GeneralLedger/ChartOfAccounts"
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

        var generalLedgerReports = new ApplicationMenuItem(
           AccountingMenus.GeneralLedgers.Name,
           l[AccountingMenus.DisplayNames.GeneralLedgers.Name],
           icon: "fas fa-chart-bar"
        );
        generalLedgerReports.AddItem(
              new ApplicationMenuItem(
                  AccountingMenus.GeneralLedgers.SingleCurrencyReport,
                  l[AccountingMenus.DisplayNames.GeneralLedgers.SingleCurrencyReport],
                  url: "/GeneralLedger/GeneralLedgers"
              ).RequirePermissions(AccountingPermissions.GeneralLedgerReports.SingleCurrencyReport)
        );
        generalLedgerReports.AddItem(
             new ApplicationMenuItem(
                 AccountingMenus.GeneralLedgers.MultipleCurrencyReport,
                 l[AccountingMenus.DisplayNames.GeneralLedgers.MultipleCurrencyReport],
                 url: "/GeneralLedger/GeneralLedgers/MultipleCurrencyReport"
             ).RequirePermissions(AccountingPermissions.GeneralLedgerReports.MultipleCurrencyReport)
        );

        generalLedgerReports.AddItem(
             new ApplicationMenuItem(
                 AccountingMenus.GeneralLedgers.MultipleCurrencyGroupReport,
                 l[AccountingMenus.DisplayNames.GeneralLedgers.MultipleCurrencyGroupReport],
                 url: "/GeneralLedger/GeneralLedgers/MultipleCurrencyGroupReport"
             ).RequirePermissions(AccountingPermissions.GeneralLedgerReports.MultipleCurrencyGroupReport)
        );

        if (generalLedgerReports.Items.Count > 0)
        {
            generalLedgerMenu.AddItem(generalLedgerReports);
        }

        var journalReportMenu = new ApplicationMenuItem(
          AccountingMenus.Journals.Name,
          l[AccountingMenus.DisplayNames.Journals.Name],
          icon: "fas fa-chart-simple"
        );
        journalReportMenu.AddItem(
              new ApplicationMenuItem(
                  AccountingMenus.Journals.SingleCurrencySortByCodeReport,
                  l[AccountingMenus.DisplayNames.Journals.SingleCurrencySortByCodeReport], 
                  url: "/GeneralLedger/Journals"
              ).RequirePermissions(AccountingPermissions.JournalReports.SingleCurrencySortByCodeReport)
        );
        journalReportMenu.AddItem(
             new ApplicationMenuItem(
                 AccountingMenus.Journals.SingleCurrencySortByDateReport,
                 l[AccountingMenus.DisplayNames.Journals.SingleCurrencySortByDateReport], 
                 url: "/GeneralLedger/Journals/SingleCurrencySortByDateReport"
             ).RequirePermissions(AccountingPermissions.JournalReports.SingleCurrencySortByDateReport)
        );

        journalReportMenu.AddItem(
            new ApplicationMenuItem(
                AccountingMenus.Journals.MultipleCurrencySortByCodeReport,
                l[AccountingMenus.DisplayNames.Journals.MultipleCurrencySortByCodeReport], 
                url: "/GeneralLedger/Journals/MultipleCurrencySortByCodeReport"
            ).RequirePermissions(AccountingPermissions.JournalReports.MultipleCurrencySortByCodeReport)
        );

        journalReportMenu.AddItem(
            new ApplicationMenuItem(
                AccountingMenus.Journals.MultipleCurrencySortByDateReport,
                l[AccountingMenus.DisplayNames.Journals.MultipleCurrencySortByDateReport], 
                url: "/GeneralLedger/Journals/MultipleCurrencySortByDateReport"
            ).RequirePermissions(AccountingPermissions.JournalReports.MultipleCurrencySortByDateReport)
        );

        if (journalReportMenu.Items.Count > 0)
        {
            generalLedgerMenu.AddItem(journalReportMenu);
        }

        var trialBalanceMenu = new ApplicationMenuItem(
            AccountingMenus.TrialBalances.Name,
            l[AccountingMenus.DisplayNames.TrialBalances.Name],
            icon: "fas fa-chart-column");

        trialBalanceMenu.AddItem(
              new ApplicationMenuItem(
                  AccountingMenus.TrialBalances.YearToDateReport,
                  l[AccountingMenus.DisplayNames.TrialBalances.YearToDateReport],
                  url: "/GeneralLedger/TrialBalances"
              ).RequirePermissions(AccountingPermissions.TrialBalanceReports.YearToDateReport));

        if (trialBalanceMenu.Items.Count > 0)
        {
            generalLedgerMenu.AddItem(trialBalanceMenu);
        }

        if (generalLedgerMenu.Items.Count > 0)
        {
            context.Menu.AddItem(generalLedgerMenu);
        }

        var receivableMenus = new ApplicationMenuItem(
          AccountingMenus.Receivable,
          l[AccountingMenus.DisplayNames.Receivable],
          icon: "fas fa-book"
        );
        receivableMenus.AddItem(
               new ApplicationMenuItem(
                   AccountingMenus.ReceivableVoucher,
                   l[AccountingMenus.DisplayNames.ReceivableVoucher],
                   icon: "fas fa-folder",
                   url: "/Receivable/ReceivableVouchers"
               ).RequirePermissions(AccountingPermissions.ReceivableVouchers.Default)
         );
        if (receivableMenus.Items.Count > 0)
        {
            context.Menu.AddItem(receivableMenus);
        }

        var payableMenus = new ApplicationMenuItem(
          AccountingMenus.Payable,
          l[AccountingMenus.DisplayNames.Payable],
          icon: "fas fa-book-open"
        );
        payableMenus.AddItem(
               new ApplicationMenuItem(
                   AccountingMenus.PayableVoucher,
                   l[AccountingMenus.DisplayNames.PayableVoucher],
                   icon: "fas fa-folder-open",
                   url: "/Payable/PayableVouchers"
               ).RequirePermissions(AccountingPermissions.PayableVouchers.Default)
         );
        if (payableMenus.Items.Count > 0)
        {
            context.Menu.AddItem(payableMenus);
        }

        return Task.CompletedTask;
    }
}
