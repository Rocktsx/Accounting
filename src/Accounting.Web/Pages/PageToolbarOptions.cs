using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Accounting.Web.Pages
{
    public static class PageToolbarOptions
    {
        public static void ConfigurePageToolbarOptions(this ServiceConfigurationContext context)
        {
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<BasicData.Currencies.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewCurrency"),
                            icon: "plus",
                            id: "newCurrency",
                            requiredPolicyName: AccountingPermissions.Currencies.Create
                        );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<BasicData.Clients.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewClient"),
                            icon: "plus",
                            id: "newCompanyBtn",
                            requiredPolicyName: AccountingPermissions.Clients.Create
                        );
                        toolbar.AddButton(
                             L("ImportClient"),
                             icon: "file-import",
                             id: "importCompanyBtn",
                             requiredPolicyName: AccountingPermissions.Clients.Import
                        );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<BasicData.Vendors.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewVendor"),
                            icon: "plus",
                            id: "newCompanyBtn",
                            requiredPolicyName: AccountingPermissions.Vendors.Create
                        );
                        toolbar.AddButton(
                             L("ImportVendor"),
                             icon: "file-import",
                             id: "importCompanyBtn",
                             requiredPolicyName: AccountingPermissions.Vendors.Import
                         );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<GeneralLedger.AccountingPeriods.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewAccountingPeriod"),
                            icon: "plus",
                            id: "newAccountingPeriodBtn",
                            requiredPolicyName: AccountingPermissions.AccountingPeriods.Create
                        );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<GeneralLedger.ChartOfAccounts.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewSubject"),
                            icon: "plus",
                            id: "newSubjectBtn",
                            requiredPolicyName: AccountingPermissions.Subjects.Create
                        );
                        toolbar.AddButton(
                          L("ImportSubject"),
                          icon: "file-import",
                          id: "importSubjectBtn",
                          requiredPolicyName: AccountingPermissions.Subjects.Import
                      );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<GeneralLedger.GeneralAccounts.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewGeneralAccount"),
                            icon: "plus",
                            id: "newGeneralAccountBtn",
                            requiredPolicyName: AccountingPermissions.GeneralAccounts.Create
                        );
                        toolbar.AddButton(
                          L("ImportGeneralAccount"),
                          icon: "file-import",
                          id: "importGeneralAccountBtn",
                          requiredPolicyName: AccountingPermissions.GeneralAccounts.Import
                      );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<GeneralLedger.SubjectCategories.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewSubjectCategory"),
                            icon: "plus",
                            id: "newSubjectCategoryBtn",
                            requiredPolicyName: AccountingPermissions.SubjectCategories.Create
                        );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<GeneralLedger.TransferVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("Search"),
                            icon: "magnifying-glass",
                            id: "searchBtn",
                            requiredPolicyName: AccountingPermissions.TransferVouchers.Default
                        );
                        toolbar.AddButton(
                          L("NewTransferVoucher"),
                          icon: "plus",
                          id: "newVoucherBtn",
                          requiredPolicyName: AccountingPermissions.TransferVouchers.Create
                        );
                        toolbar.AddButton(
                             L("ImportTransferVoucher"),
                             icon: "file-import",
                             id: "importTVBtn",
                             requiredPolicyName: AccountingPermissions.TransferVouchers.Import
                         );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<GeneralLedger.VoucherStates.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("UpdateStatus"),
                            icon: "pencil",
                            id: "updateStatusBtn",
                            requiredPolicyName: AccountingPermissions.VoucherStates.UpdateStatus
                        );
                    });
            });
            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<Receivable.ReceivableVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("Search"),
                            icon: "magnifying-glass",
                            id: "searchBtn",
                            requiredPolicyName: AccountingPermissions.ReceivableVouchers.Default
                        );
                        toolbar.AddButton(
                          L("NewReceivableVoucher"),
                          icon: "plus",
                          id: "newVoucherBtn",
                          requiredPolicyName: AccountingPermissions.ReceivableVouchers.Create
                        );
                    });
            });

            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<Payable.PayableVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("Search"),
                            icon: "magnifying-glass",
                            id: "searchBtn",
                            requiredPolicyName: AccountingPermissions.PayableVouchers.Default
                        );
                        toolbar.AddButton(
                          L("NewPayableVoucher"),
                          icon: "plus",
                          id: "newVoucherBtn",
                          requiredPolicyName: AccountingPermissions.PayableVouchers.Create
                        );
                    });
            });

            context.Services.Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<GeneralLedger.GeneralLedgerReports.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("Search"),
                            icon: "magnifying-glass",
                            id: "searchBtn",
                            requiredPolicyName: AccountingPermissions.GeneralLedgerReports.SingleCurrencyReport
                        ); 
                    });
            });

        }
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AccountingResource>(name);
        }
    }
}
