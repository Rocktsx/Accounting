using Accounting.Localization;
using JetBrains.Annotations;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Accounting.Permissions;

public class AccountingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        AddPermissionGroup(context, AccountingPermissions.Currency, nameof(AccountingPermissions.Currency),
            AccountingPermissions.CurrencyCreation, AccountingPermissions.CurrencyDeletion,
            AccountingPermissions.CurrencyEdit);
        AddPermissionGroup(context, AccountingPermissions.Client, nameof(AccountingPermissions.Client),
            AccountingPermissions.ClientCreation, AccountingPermissions.ClientDeletion,
            AccountingPermissions.ClientEdit);
        AddPermissionGroup(context, AccountingPermissions.Vendor, nameof(AccountingPermissions.Vendor),
            AccountingPermissions.VendorCreation, AccountingPermissions.VendorDeletion,
            AccountingPermissions.VendorEdit);
        AddPermissionGroup(context, AccountingPermissions.AccountingPeriod,
            nameof(AccountingPermissions.AccountingPeriod), AccountingPermissions.AccountingPeriodCreation,
            AccountingPermissions.AccountingPeriodDeletion, AccountingPermissions.AccountingPeriodEdit);
        AddPermissionGroup(context, AccountingPermissions.AccountType, nameof(AccountingPermissions.AccountType),
            AccountingPermissions.AccountTypeCreation, AccountingPermissions.AccountTypeDeletion,
            AccountingPermissions.AccountTypeEdit);
        AddPermissionGroup(context, AccountingPermissions.SubjectCategory,
            nameof(AccountingPermissions.SubjectCategory), AccountingPermissions.SubjectCategoryCreation,
            AccountingPermissions.SubjectCategoryDeletion, AccountingPermissions.SubjectCategoryEdit);
        AddPermissionGroup(context, AccountingPermissions.Subject, nameof(AccountingPermissions.Subject),
            AccountingPermissions.SubjectCreation, AccountingPermissions.SubjectDeletion,
            AccountingPermissions.SubjectEdit);

        var setttingDisplay =
            L(AccountingPermissions.PermissionPrefix + nameof(AccountingPermissions.AccountingSetting));
        context.AddGroup(AccountingPermissions.AccountingSetting, setttingDisplay)
            .AddPermission(AccountingPermissions.AccountingSetting, setttingDisplay);
    }

    private static void AddPermission(PermissionGroupDefinition group, string permissionName,
        LocalizableString permissionDisplayName, string creationPermission, string deletionPermissin,
        string editPermission)
    {
        var permission = group.AddPermission(permissionName, permissionDisplayName);
        permission.AddChild(creationPermission, L(AccountingPermissions.CreationDisplayName));
        permission.AddChild(deletionPermissin, L(AccountingPermissions.DeletionDisplayName));
        permission.AddChild(editPermission, L(AccountingPermissions.EditDisplayName));
    }

    private static void AddPermissionGroup(IPermissionDefinitionContext context, string permission,
        LocalizableString permissionName, string creationPermission, string deletionPermissin, string editPermission)
    {
        var group = context.AddGroup(permission, permissionName);
        AddPermission(group, permission, permissionName, creationPermission, deletionPermissin, editPermission);
    }

    private static void AddPermissionGroup(IPermissionDefinitionContext context, string permission,
        string permissionName, string creationPermission, string deletionPermissin, string editPermission)
    {
        var permissionDisplayName = L(AccountingPermissions.PermissionPrefix + permissionName);
        AddPermissionGroup(context, permission, permissionDisplayName, creationPermission, deletionPermissin,
            editPermission);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountingResource>(name);
    }
}