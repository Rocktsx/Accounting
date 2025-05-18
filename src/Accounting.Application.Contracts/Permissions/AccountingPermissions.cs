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
}
