namespace Accounting.Permissions;

public static class AccountingPermissions
{
    public const string PermissionPrefix = "Permission:";
    public const string GroupName = "Accounting";

    public const string Creation = ".Creation";
    public const string Deletion = ".Deletion";
    public const string Edit = ".Edit";
    public const string CreationDisplayName = PermissionPrefix+"Creation";
    public const string DeletionDisplayName = PermissionPrefix + "Deletion";
    public const string EditDisplayName = PermissionPrefix + "Edit";

    public const string BasicDataGroupName = GroupName + ".BasicData";

    public const string Currency = BasicDataGroupName + ".Currency";
    public const string CurrencyCreation = Currency + Creation;
    public const string CurrencyDeletion = Currency + Deletion;
    public const string CurrencyEdit = Currency + Edit;

    public const string Client = BasicDataGroupName + ".Client";
    public const string ClientCreation = Client + Creation;
    public const string ClientDeletion = Client + Deletion;
    public const string ClientEdit = Client + Edit;

    public const string Vendor = BasicDataGroupName + ".Vendor";
    public const string VendorCreation = Vendor + Creation;
    public const string VendorDeletion = Vendor + Deletion;
    public const string VendorEdit = Vendor + Edit;

    public const string GeneralLedgerGroupName = GroupName + ".GeneralLedger";
    public const string AccountingPeriod = GeneralLedgerGroupName + ".AccountingPeriod";
    public const string AccountingPeriodCreation = AccountingPeriod + Creation;
    public const string AccountingPeriodDeletion = AccountingPeriod + Deletion;
    public const string AccountingPeriodEdit = AccountingPeriod + Edit;

    /// <summary>
    /// 总账类别
    /// </summary>
    public const string GeneralAccount = GeneralLedgerGroupName + ".GeneralAccount";
    public const string GeneralAccountCreation = GeneralAccount + Creation;
    public const string GeneralAccountDeletion = GeneralAccount + Deletion;
    public const string GeneralAccountEdit = GeneralAccount + Edit;
    
    /// <summary>
    /// 科目类别
    /// </summary>
    public const string SubjectCategory = GeneralLedgerGroupName + ".SubjectCategory";
    public const string SubjectCategoryCreation = SubjectCategory + Creation;
    public const string SubjectCategoryDeletion = SubjectCategory + Deletion;
    public const string SubjectCategoryEdit = SubjectCategory + Edit;

    public const string Subject = GeneralLedgerGroupName + ".Subject";
    public const string SubjectCreation = Subject + Creation;
    public const string SubjectDeletion = Subject + Deletion;
    public const string SubjectEdit = Subject + Edit;
    
    public const string AccountingSetting = GroupName + ".AccountingSetting";
}
