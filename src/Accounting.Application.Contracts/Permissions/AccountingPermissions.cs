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
    /// 总账类别
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
    /// 转账传票
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

    public class GeneralLedgerReports
    {
        public const string Name = "GeneralLedgerReport";
        public const string Default = GeneralLedgerGroup + Dot + Name;
        public const string SingleCurrencyReportText = "SingleCurrencyReport";
        public const string SingleCurrencyReport = Default + Dot + SingleCurrencyReportText;
        public const string SingleCurrencyReportDisplayName = PermissionPrefix + Name + Dot + SingleCurrencyReportText;
    }

    /// <summary>
    /// 应收账组
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
    /// 应付账组
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