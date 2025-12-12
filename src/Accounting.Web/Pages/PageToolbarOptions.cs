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
                    }
                );

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
                    }
                );

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
                    }
                );

                options.Configure<GeneralLedger.AccountingPeriods.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewAccountingPeriod"),
                            icon: "plus",
                            id: "newAccountingPeriodBtn",
                            requiredPolicyName: AccountingPermissions.AccountingPeriods.Create
                        );
                    }
                );

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
                    }
                );

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
                    }
                );

                options.Configure<GeneralLedger.SubjectCategories.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewSubjectCategory"),
                            icon: "plus",
                            id: "newSubjectCategoryBtn",
                            requiredPolicyName: AccountingPermissions.SubjectCategories.Create
                        );
                    }
                );

                options.Configure<GeneralLedger.TransferVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.TransferVouchers.Default);
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
                    }
                );


                options.Configure<GeneralLedger.VoucherStates.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("UpdateStatus"),
                            icon: "pencil",
                            id: "updateStatusBtn",
                            requiredPolicyName: AccountingPermissions.VoucherStates.UpdateStatus
                        );
                    }
                );


                options.Configure<Receivable.ReceivableVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.ReceivableVouchers.Default);
                        toolbar.AddButton(
                          L("NewReceivableVoucher"),
                          icon: "plus",
                          id: "newVoucherBtn",
                          requiredPolicyName: AccountingPermissions.ReceivableVouchers.Create
                        );
                    }
                );

                options.Configure<Payable.PayableVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.PayableVouchers.Default); 
                        toolbar.AddButton(
                          L("NewPayableVoucher"),
                          icon: "plus",
                          id: "newVoucherBtn",
                          requiredPolicyName: AccountingPermissions.PayableVouchers.Create
                        );
                    }
                );

                options.Configure<GeneralLedger.GeneralLedgers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.GeneralLedgerReports.SingleCurrencyReport);
                    }
                );
                options.Configure<GeneralLedger.GeneralLedgers.MultipleCurrencyReportModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.GeneralLedgerReports.MultipleCurrencyReport);
                    }
                );

                options.Configure<GeneralLedger.GeneralLedgers.MultipleCurrencyGroupReportModel>(
                   toolbar =>
                   {
                       toolbar.AddSearchButton(AccountingPermissions.GeneralLedgerReports.MultipleCurrencyGroupReport);
                   }
               );

                options.Configure<GeneralLedger.Journals.IndexModel>(
                   toolbar =>
                   {
                       toolbar.AddSearchButton(AccountingPermissions.JournalReports.SingleCurrencySortByCodeReport);
                   }
               );

            });
        }

        private static void AddSearchButton(this PageToolbar toolbar, string permission)
        {
            toolbar.AddButton(
                L("Search"),
                icon: "magnifying-glass",
                id: "searchBtn",
                requiredPolicyName: permission
            );
        }
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AccountingResource>(name);
        }
    }
}
