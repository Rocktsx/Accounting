using Accounting.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using static Accounting.Permissions.AccountingPermissions;

namespace Accounting.Permissions;

public class AccountingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        AddPermissionGroup(context, Currencies.Default, Currencies.Name, Currencies.Create, Currencies.Delete, Currencies.Update);

        var group = AddPermissionGroup(context, Clients.Default, Clients.Name, Clients.Create, Clients.Delete, Clients.Update);
        group.Permissions[0].AddChild(Clients.Import, L(ImportDisplayName));

        group = AddPermissionGroup(context, Vendors.Default, Vendors.Name, Vendors.Create, Vendors.Delete, Vendors.Update);
        group.Permissions[0].AddChild(Vendors.Import, L(ImportDisplayName));

        AddPermissionGroup(context, AccountingPeriods.Default, AccountingPeriods.Name,
            AccountingPeriods.Create, AccountingPeriods.Delete, AccountingPeriods.Update);

        group = AddPermissionGroup(context, GeneralAccounts.Default, GeneralAccounts.Name,
            GeneralAccounts.Create, GeneralAccounts.Delete, GeneralAccounts.Update);
        group.Permissions[0].AddChild(GeneralAccounts.Import, L(ImportDisplayName));

        AddPermissionGroup(context, SubjectCategories.Default, SubjectCategories.Name,
            SubjectCategories.Create, SubjectCategories.Delete, SubjectCategories.Update);

        group = AddPermissionGroup(context, Subjects.Default, Subjects.Name, Subjects.Create, Subjects.Delete, Subjects.Update);
        group.Permissions[0].AddChild(Subjects.Import, L(ImportDisplayName));

        var setttingDisplay = L(PermissionPrefix + nameof(AccountingSetting));
        context.AddGroup(AccountingSetting, setttingDisplay)
            .AddPermission(AccountingSetting, setttingDisplay);

        group = AddPermissionGroup(context, TransferVouchers.Default, TransferVouchers.Name,
           TransferVouchers.Create, TransferVouchers.Delete, TransferVouchers.Update);
        group.Permissions[0].AddChild(TransferVouchers.UpdateStatus, L(UpdateStatusDisplayName));
        group.Permissions[0].AddChild(TransferVouchers.Import, L(ImportDisplayName));

        var voucherStatesName = PermissionPrefix + VoucherStates.Name;
        group = context.AddGroup(VoucherStates.Default, L(voucherStatesName));
        var permission = group.AddPermission(VoucherStates.Default, L(voucherStatesName));
        permission.AddChild(VoucherStates.UpdateStatus, L(UpdateStatusDisplayName));

        group = AddPermissionGroup(context, ReceivableVouchers.Default, ReceivableVouchers.Name,
           ReceivableVouchers.Create, ReceivableVouchers.Delete, ReceivableVouchers.Update);
        group.Permissions[0].AddChild(ReceivableVouchers.UpdateStatus, L(UpdateStatusDisplayName));

        group = AddPermissionGroup(context, PayableVouchers.Default, PayableVouchers.Name,
         PayableVouchers.Create, PayableVouchers.Delete, PayableVouchers.Update);
        group.Permissions[0].AddChild(PayableVouchers.UpdateStatus, L(UpdateStatusDisplayName));

        group = context.AddGroup(GeneralLedgerReports.Default, L(PermissionPrefix + GeneralLedgerReports.Name));
        group.AddPermission(GeneralLedgerReports.SingleCurrencyReport, L(GeneralLedgerReports.SingleCurrencyReportName));
        group.AddPermission(GeneralLedgerReports.MultipleCurrencyReport, L(GeneralLedgerReports.MultipleCurrencyReportName));
        group.AddPermission(GeneralLedgerReports.MultipleCurrencyGroupReport, L(GeneralLedgerReports.MultipleCurrencyGroupReportName));

        group = context.AddGroup(JournalReports.Default, L(PermissionPrefix + JournalReports.Name));
        group.AddPermission(JournalReports.SingleCurrencySortByCodeReport, L(JournalReports.SingleCurrencySortByCodeReport));
        group.AddPermission(JournalReports.SingleCurrencySortByDateReport, L(JournalReports.SingleCurrencySortByDateReportName));
        group.AddPermission(JournalReports.MultipleCurrencySortByCodeReport, L(JournalReports.MultipleCurrencySortByCodeReportName));
        group.AddPermission(JournalReports.MultipleCurrencySortByDateReport, L(JournalReports.MultipleCurrencySortByDateReportName));

        group = context.AddGroup(TrialBalanceReports.Default, L(PermissionPrefix + TrialBalanceReports.Name));
        group.AddPermission(TrialBalanceReports.MonthToDateYearToDateReport, L(TrialBalanceReports.MonthToDateYearToDateReportName));
        group.AddPermission(TrialBalanceReports.YearToDateReport, L(TrialBalanceReports.YearToDateReportName));

        group = context.AddGroup(ProfitAndLossReports.Default, L(PermissionPrefix + ProfitAndLossReports.Name));
        group.AddPermission(ProfitAndLossReports.MonthToDateYearToDateReport, L(ProfitAndLossReports.MonthToDateYearToDateReportName));
        group.AddPermission(ProfitAndLossReports.YearToDateReport, L(ProfitAndLossReports.YearToDateReportName));
        group.AddPermission(ProfitAndLossReports.TwelveMonthsReport, L(ProfitAndLossReports.TwelveMonthsReportName));

        group = context.AddGroup(BalanceSheetReports.Default, L(PermissionPrefix + BalanceSheetReports.Name));
        group.AddPermission(BalanceSheetReports.MonthToDateYearToDateReport, L(BalanceSheetReports.MonthToDateYearToDateReportName));
        group.AddPermission(BalanceSheetReports.YearToDateReport, L(BalanceSheetReports.YearToDateReportName));

        group = context.AddGroup(ReceivableAgingReports.Default, L(PermissionPrefix + ReceivableAgingReports.Name));
        group.AddPermission(ReceivableAgingReports.AgingSummarySingleCurrency, L(ReceivableAgingReports.AgingSummarySingleCurrencyName));
        group.AddPermission(ReceivableAgingReports.AgingSummaryMultipleCurrency, L(ReceivableAgingReports.AgingSummaryMultipleCurrencyName));
    }

    private static PermissionDefinition AddPermission(PermissionGroupDefinition group, string permissionName,
        LocalizableString permissionDisplayName, string creationPermission, string deletionPermissin, string editPermission)
    {
        var permission = group.AddPermission(permissionName, permissionDisplayName);
        permission.AddChild(creationPermission, L(CreationDisplayName));
        permission.AddChild(deletionPermissin, L(DeletionDisplayName));
        permission.AddChild(editPermission, L(EditDisplayName));

        return permission;
    }

    private static PermissionGroupDefinition AddPermissionGroup(IPermissionDefinitionContext context, string permission,
        LocalizableString permissionName, string creationPermission, string deletionPermissin, string editPermission)
    {
        var group = context.AddGroup(permission, permissionName);
        AddPermission(group, permission, permissionName, creationPermission, deletionPermissin, editPermission);

        return group;
    }

    private static PermissionGroupDefinition AddPermissionGroup(IPermissionDefinitionContext context, string permission,
        string permissionName, string creationPermission, string deletionPermissin, string editPermission)
    {
        var permissionDisplayName = L(PermissionPrefix + permissionName);
        return AddPermissionGroup(context, permission, permissionDisplayName, creationPermission, deletionPermissin,
             editPermission);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountingResource>(name);
    }
}