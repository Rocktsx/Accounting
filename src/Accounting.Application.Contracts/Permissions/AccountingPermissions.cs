namespace Accounting.Permissions;

public static class AccountingPermissions
{
    public const string PermissionPrefix = "Permission:";
    public const string GroupName = "Accounting";

    private const string Dot = ".";
    public const string CreationText = "Creation";
    public const string DeletionText = "Deletion";
    public const string EditText = "Edit";
    public const string ImportText = "Import";
    public const string ExportText = "Export";
    public const string UpdateStatusText = "UpdateStatus";
    public const string PrintText = "Print";

    public const string Creation = Dot + CreationText;
    public const string Deletion = Dot + DeletionText;
    public const string Edit = Dot + EditText;
    public const string Imports = Dot + ImportText;
    public const string Exports = Dot + ExportText;
    public const string UpdateStatusDot = Dot + UpdateStatusText;

    public const string CreationDisplayName = PermissionPrefix + CreationText;
    public const string DeletionDisplayName = PermissionPrefix + DeletionText;
    public const string EditDisplayName = PermissionPrefix + EditText;
    public const string PrintDisplayName = PermissionPrefix + PrintText;
    public const string ImportDisplayName = PermissionPrefix + ImportText;
    public const string ExportDisplayName = PermissionPrefix + ExportText;
    public const string UpdateStatusDisplayName = PermissionPrefix + UpdateStatusText;

    public const string BasicDataGroup = GroupName + ".BasicData";

    public class Currencies
    {
        public const string Name = "Currency";
        public const string Default = BasicDataGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
    }
    public class Clients
    {
        public const string Name = "Client";
        public const string Default = BasicDataGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
        public const string Import = Default + Imports;
    }
    public class Vendors
    {
        public const string Name = "Vendor";
        public const string Default = BasicDataGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
        public const string Import = Default + Imports;
    }

    public const string GeneralLedgerGroup = GroupName + ".GeneralLedger";
    /// <summary>
    /// 会计年度
    /// </summary>
    public class AccountingPeriods
    {
        public const string Name = "AccountingPeriod";
        public const string Default = GeneralLedgerGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
    }

    /// <summary>
    /// 总帐类别
    /// </summary>
    public class GeneralAccounts
    {
        public const string Name = "GeneralAccount";
        public const string Default = GeneralLedgerGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
        public const string Import = Default + Imports;
    }
    /// <summary>
    /// 科目类别
    /// </summary>
    public class SubjectCategories
    {
        public const string Name = "SubjectCategory";
        public const string Default = GeneralLedgerGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
    }
    /// <summary>
    /// 科目
    /// </summary>
    public class Subjects
    {
        public const string Name = "Subject";
        public const string Default = GeneralLedgerGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
        public const string Import = Default + Imports;
    }

    public const string AccountingSetting = GroupName + ".AccountingSetting";

    /// <summary>
    /// 转帐传票
    /// </summary>
    public class TransferVouchers
    {
        public const string Name = "TransferVoucher";
        public const string Default = GeneralLedgerGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
        public const string UpdateStatus = Default + UpdateStatusDot;
        public const string Import = Default + Imports;
    }

    /// <summary>
    /// 传票状态
    /// </summary>
    public class VoucherStates
    {
        public const string Name = "VoucherStates";
        public const string Default = GeneralLedgerGroup + Dot + Name;
        public const string UpdateStatus = Default + UpdateStatusDot;
    }

    /// <summary>
    /// 总帐报表
    /// </summary>
    public class GeneralLedgerReports
    {
        public const string Name = "GeneralLedgerReport";
        public const string Default = GeneralLedgerGroup + Dot + Name;

        private const string SingleCurrencyReportText = "SingleCurrencyReport";
        internal const string SingleCurrencyReportName = PermissionPrefix + Name + Dot + SingleCurrencyReportText;
        public const string SingleCurrencyReport = Default + Dot + SingleCurrencyReportText;

        private const string MultipleCurrencyReportText = "MultipleCurrencyReport";
        internal const string MultipleCurrencyReportName = PermissionPrefix + Name + Dot + MultipleCurrencyReportText;
        public const string MultipleCurrencyReport = Default + Dot + MultipleCurrencyReportText;

        private const string MultipleCurrencyGroupReportText = "MultipleCurrencyGroupReport";
        internal const string MultipleCurrencyGroupReportName = PermissionPrefix + Name + Dot + MultipleCurrencyGroupReportText;
        public const string MultipleCurrencyGroupReport = Default + Dot + MultipleCurrencyGroupReportText;
    }

    /// <summary>
    /// 日志帐报表
    /// </summary>
    public class JournalReports
    {
        public const string Name = "JournalReport";
        public const string Default = GeneralLedgerGroup + Dot + Name;

        private const string SingleCurrencySortByCodeReportText = "SingleCurrencySortByCodeReport";
        internal const string SingleCurrencySortByCodeReportName = PermissionPrefix + Name + Dot + SingleCurrencySortByCodeReportText;
        public const string SingleCurrencySortByCodeReport = Default + Dot + SingleCurrencySortByCodeReportText;

        private const string SingleCurrencySortByDateReportText = "SingleCurrencySortByDateReport";
        internal const string SingleCurrencySortByDateReportName = PermissionPrefix + Name + Dot + SingleCurrencySortByDateReportText;
        public const string SingleCurrencySortByDateReport = Default + Dot + SingleCurrencySortByDateReportText;

        private const string MultipleCurrencySortByCodeReportText = "MultipleCurrencySortByCodeReport";
        internal const string MultipleCurrencySortByCodeReportName = PermissionPrefix + Name + Dot + MultipleCurrencySortByCodeReportText;
        public const string MultipleCurrencySortByCodeReport = Default + Dot + MultipleCurrencySortByCodeReportText;

        private const string MultipleCurrencySortByDateReportText = "MultipleCurrencySortByDateReport";
        internal const string MultipleCurrencySortByDateReportName = PermissionPrefix + Name + Dot + MultipleCurrencySortByDateReportText;
        public const string MultipleCurrencySortByDateReport = Default + Dot + MultipleCurrencySortByDateReportText;
    }

    /// <summary>
    /// 试算表报表
    /// </summary>
    public class TrialBalanceReports
    {
        public const string Name = "TrialBalanceReport";
        public const string Default = GeneralLedgerGroup + Dot + Name;

        private const string YearToDateReportText = "YearToDateReport";
        internal const string YearToDateReportName = PermissionPrefix + Name + Dot + YearToDateReportText;
        public const string YearToDateReport = Default + Dot + YearToDateReportText;

        private const string MonthToDateYearToDateReportText = "MonthToDateYearToDateReport";
        internal const string MonthToDateYearToDateReportName = PermissionPrefix + Name + Dot + MonthToDateYearToDateReportText;
        public const string MonthToDateYearToDateReport = Default + Dot + MonthToDateYearToDateReportText;
    }

    /// <summary>
    /// 应收帐组
    /// </summary>
    public const string ReceivableGroup = GroupName + Dot + "Receivable";

    /// <summary>
    /// 收款传票
    /// </summary>
    public class ReceivableVouchers
    {
        public const string Name = "ReceivableVoucher";
        public const string Default = ReceivableGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
        public const string UpdateStatus = Default + UpdateStatusDot;
    }

    /// <summary>
    /// 应付帐组
    /// </summary>
    public const string PayableGroup = GroupName + Dot + "Payable";

    /// <summary>
    /// 付款传票
    /// </summary>
    public class PayableVouchers
    {
        public const string Name = "PayableVoucher";
        public const string Default = PayableGroup + Dot + Name;
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
        public const string UpdateStatus = Default + UpdateStatusDot;
    }
}