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
                            requiredPolicyName: AccountingPermissions.Currencies.Create);
                    });

                options.Configure<BasicData.Clients.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewClient"),
                            icon: "plus",
                            id: "newCompanyBtn",
                            requiredPolicyName: AccountingPermissions.Clients.Create);

                        toolbar.AddButton(
                             L("ImportClient"),
                             icon: "file-import",
                             id: "importCompanyBtn",
                             requiredPolicyName: AccountingPermissions.Clients.Import);
                    });

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
                             requiredPolicyName: AccountingPermissions.Vendors.Import);
                    });

                options.Configure<GeneralLedger.AccountingPeriods.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewAccountingPeriod"),
                            icon: "plus",
                            id: "newAccountingPeriodBtn",
                            requiredPolicyName: AccountingPermissions.AccountingPeriods.Create);
                    });

                options.Configure<GeneralLedger.ChartOfAccounts.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewSubject"),
                            icon: "plus",
                            id: "newSubjectBtn",
                            requiredPolicyName: AccountingPermissions.Subjects.Create);

                        toolbar.AddButton(
                          L("ImportSubject"),
                          icon: "file-import",
                          id: "importSubjectBtn",
                          requiredPolicyName: AccountingPermissions.Subjects.Import);
                    });

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
                          requiredPolicyName: AccountingPermissions.GeneralAccounts.Import);
                    });

                options.Configure<GeneralLedger.SubjectCategories.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("NewSubjectCategory"),
                            icon: "plus",
                            id: "newSubjectCategoryBtn",
                            requiredPolicyName: AccountingPermissions.SubjectCategories.Create);
                    });

                options.Configure<GeneralLedger.TransferVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.TransferVouchers.Default);
                        toolbar.AddButton(
                          L("NewTransferVoucher"),
                          icon: "plus",
                          id: "newVoucherBtn",
                          requiredPolicyName: AccountingPermissions.TransferVouchers.Create);

                        toolbar.AddButton(
                             L("ImportTransferVoucher"),
                             icon: "file-import",
                             id: "importTVBtn",
                             requiredPolicyName: AccountingPermissions.TransferVouchers.Import);
                    });


                options.Configure<GeneralLedger.VoucherStates.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            L("UpdateStatus"),
                            icon: "pencil",
                            id: "updateStatusBtn",
                            requiredPolicyName: AccountingPermissions.VoucherStates.UpdateStatus);
                    });


                options.Configure<Receivable.ReceivableVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.ReceivableVouchers.Default);
                        toolbar.AddButton(
                          L("NewReceivableVoucher"),
                          icon: "plus",
                          id: "newVoucherBtn",
                          requiredPolicyName: AccountingPermissions.ReceivableVouchers.Create);
                    });

                options.Configure<Payable.PayableVouchers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.PayableVouchers.Default);
                        toolbar.AddButton(
                          L("NewPayableVoucher"),
                          icon: "plus",
                          id: "newVoucherBtn",
                          requiredPolicyName: AccountingPermissions.PayableVouchers.Create);
                    });

                options.Configure<GeneralLedger.GeneralLedgers.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchAndPrintButton(AccountingPermissions.GeneralLedgerReports.SingleCurrencyReport);
                    });

                options.Configure<GeneralLedger.GeneralLedgers.MultipleCurrencyReportModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchAndPrintButton(AccountingPermissions.GeneralLedgerReports.MultipleCurrencyReport);
                    });

                options.Configure<GeneralLedger.GeneralLedgers.MultipleCurrencyGroupReportModel>(
                   toolbar =>
                   {
                       toolbar.AddSearchAndPrintButton(AccountingPermissions.GeneralLedgerReports.MultipleCurrencyGroupReport);
                   });

                options.Configure<GeneralLedger.Journals.IndexModel>(
                   toolbar =>
                   {
                       toolbar.AddSearchButton(AccountingPermissions.JournalReports.SingleCurrencySortByCodeReport);
                   });

                options.Configure<GeneralLedger.Journals.SingleCurrencySortByDateReport>(
                     toolbar =>
                     {
                         toolbar.AddSearchButton(AccountingPermissions.JournalReports.SingleCurrencySortByDateReport);
                     });

                options.Configure<GeneralLedger.Journals.MultipleCurrencySortByCodeReportModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.JournalReports.MultipleCurrencySortByCodeReport);
                    });

                options.Configure<GeneralLedger.Journals.MultipleCurrencySortByDateReportModel>(
                   toolbar =>
                   {
                       toolbar.AddSearchButton(AccountingPermissions.JournalReports.MultipleCurrencySortByDateReport);
                   });

                options.Configure<GeneralLedger.TrialBalances.IndexModel>(
                   toolbar =>
                   {
                       toolbar.AddSearchButton(AccountingPermissions.TrialBalanceReports.YearToDateReport);
                   });

                options.Configure<GeneralLedger.TrialBalances.MtdYtdReportModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.TrialBalanceReports.MonthToDateYearToDateReport);
                  });

                options.Configure<GeneralLedger.IncomeStatements.IndexModel>(
                   toolbar =>
                   {
                       toolbar.AddSearchButton(AccountingPermissions.ProfitAndLossReports.YearToDateReport);
                   });

                options.Configure<GeneralLedger.IncomeStatements.MtdYtdReportModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.ProfitAndLossReports.MonthToDateYearToDateReport);
                  });

                options.Configure<GeneralLedger.IncomeStatements.TwelveMonthsReportModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.ProfitAndLossReports.TwelveMonthsReport);
                  });

                options.Configure<GeneralLedger.BalanceSheets.IndexModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.BalanceSheetReports.YearToDateReport);
                  });

                options.Configure<GeneralLedger.BalanceSheets.MtdYtdReportModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.BalanceSheetReports.MonthToDateYearToDateReport);
                  });

                options.Configure<Receivable.DebtorAgingReports.IndexModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.ReceivableAgingReports.AgingSummarySingleCurrency);
                  });

                options.Configure<Receivable.DebtorAgingReports.SummaryMultipleCurrencyReportModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.ReceivableAgingReports.AgingSummaryMultipleCurrency);
                  });

                options.Configure<Receivable.DebtorAgingReports.DetailReportModel>(
                 toolbar =>
                 {
                     toolbar.AddSearchButton(AccountingPermissions.ReceivableAgingReports.AgingDetail);
                 });

                options.Configure<Payable.CreditorAgingReports.IndexModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.PayableAgingReports.AgingSummarySingleCurrency);
                  });

                options.Configure<Payable.CreditorAgingReports.SummaryMultipleCurrencyReportModel>(
                  toolbar =>
                  {
                      toolbar.AddSearchButton(AccountingPermissions.PayableAgingReports.AgingSummaryMultipleCurrency);
                  });

                options.Configure<Payable.CreditorAgingReports.DetailReportModel>(
                 toolbar =>
                 {
                     toolbar.AddSearchButton(AccountingPermissions.PayableAgingReports.AgingDetail);
                 });

                options.Configure<GeneralLedger.BankReconciliations.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddSearchButton(AccountingPermissions.BankReconciliations.Default);
                        toolbar.AddButton(
                            L("Save"),
                            icon: "check",
                            id: "saveBtn",
                            requiredPolicyName: AccountingPermissions.BankReconciliations.Update);
                    });

                options.Configure<GeneralLedger.BankReconciliationReports.IndexModel>(
                 toolbar =>
                 {
                     toolbar.AddSearchButton(AccountingPermissions.BankReconciliationReports.Report);
                 });

                options.Configure<GeneralLedger.BankReconciliationReports.UnpresentedReportModel>(
                toolbar =>
                {
                    toolbar.AddSearchButton(AccountingPermissions.BankReconciliationReports.UnpresentedReport);
                });
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
        private static void AddSearchAndPrintButton(this PageToolbar toolbar, string searchPermission, string printPermission = "")
        {
            toolbar.AddSearchButton(searchPermission);
            toolbar.AddButton(
                L("Print"),
                icon: "print",
                id: "printBtn",
                requiredPolicyName: string.IsNullOrEmpty(printPermission) ? searchPermission : printPermission
            );
        }
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AccountingResource>(name);
        }
    }
}
