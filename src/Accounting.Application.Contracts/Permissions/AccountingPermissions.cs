namespace Accounting.Permissions;

public static class AccountingPermissions
{
    public const string PermissionPrefix = "Permission:";
    public const string GroupName = "Accounting";

    public const string Creation = ".Creation";
    public const string Deletion = ".Deletion";
    public const string Edit = ".Edit";
    public const string CreationDisplayName = PermissionPrefix + "Creation";
    public const string DeletionDisplayName = PermissionPrefix + "Deletion";
    public const string EditDisplayName = PermissionPrefix + "Edit";
    private const string Dot = ".";

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
    }
    public class Vendors
    {
        public const string Name = "Vendor";
        public const string Default = BasicDataGroup + Dot + Name; 
        public const string Create = Default + Creation;
        public const string Delete = Default + Deletion;
        public const string Update = Default + Edit;
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
    } 
}