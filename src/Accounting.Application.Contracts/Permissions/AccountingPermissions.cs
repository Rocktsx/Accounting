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

    public const string GenenalLedgerGroupName = GroupName + ".GenenalLedger";
    public const string AccountingPeriod = GenenalLedgerGroupName + ".AccountingPeriod";
    public const string AccountingPeriodCreation = AccountingPeriod + Creation;
    public const string AccountingPeriodDeletion = AccountingPeriod + Deletion;
    public const string AccountingPeriodEdit = AccountingPeriod + Edit;

    public const string AccountType = GenenalLedgerGroupName + ".AccountType";
    public const string AccountTypeCreation = AccountType + Creation;
    public const string AccountTypeDeletion = AccountType + Deletion;
    public const string AccountTypeEdit = AccountType + Edit;

    public const string SubjectCategory = GenenalLedgerGroupName + ".SubjectCategory";
    public const string SubjectCategoryCreation = SubjectCategory + Creation;
    public const string SubjectCategoryDeletion = SubjectCategory + Deletion;
    public const string SubjectCategoryEdit = SubjectCategory + Edit;

    public const string Subject = GenenalLedgerGroupName + ".Subject";
    public const string SubjectCreation = Subject + Creation;
    public const string SubjectDeletion = Subject + Deletion;
    public const string SubjectEdit = Subject + Edit;
    
    public const string AccountingSetting = GroupName + ".AccountingSetting";
}
