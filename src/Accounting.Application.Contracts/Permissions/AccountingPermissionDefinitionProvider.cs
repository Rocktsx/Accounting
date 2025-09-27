using Accounting.Localization;
using JetBrains.Annotations;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using static Accounting.Permissions.AccountingPermissions;

namespace Accounting.Permissions;

public class AccountingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        AddPermissionGroup(context, AccountingPermissions.Currencies.Default, AccountingPermissions.Currencies.Name,
            AccountingPermissions.Currencies.Create, AccountingPermissions.Currencies.Delete,
            AccountingPermissions.Currencies.Update);

        AddPermissionGroup(context, AccountingPermissions.Clients.Default, AccountingPermissions.Clients.Name,
            AccountingPermissions.Clients.Create, AccountingPermissions.Clients.Delete,
            AccountingPermissions.Clients.Update);

        AddPermissionGroup(context, AccountingPermissions.Vendors.Default, AccountingPermissions.Vendors.Name,
            AccountingPermissions.Vendors.Create, AccountingPermissions.Vendors.Delete,
            AccountingPermissions.Vendors.Update);

        AddPermissionGroup(context, AccountingPermissions.AccountingPeriods.Default, AccountingPermissions.AccountingPeriods.Name,
            AccountingPermissions.AccountingPeriods.Create, AccountingPermissions.AccountingPeriods.Delete, AccountingPermissions.AccountingPeriods.Update);

        AddPermissionGroup(context, AccountingPermissions.GeneralAccounts.Default, AccountingPermissions.GeneralAccounts.Name,
            AccountingPermissions.GeneralAccounts.Create, AccountingPermissions.GeneralAccounts.Delete, AccountingPermissions.GeneralAccounts.Update);

        AddPermissionGroup(context, AccountingPermissions.SubjectCategories.Default, AccountingPermissions.SubjectCategories.Name,
            AccountingPermissions.SubjectCategories.Create, AccountingPermissions.SubjectCategories.Delete, AccountingPermissions.SubjectCategories.Update);

        AddPermissionGroup(context, AccountingPermissions.Subjects.Default, AccountingPermissions.Subjects.Name,
           AccountingPermissions.Subjects.Create, AccountingPermissions.Subjects.Delete, AccountingPermissions.Subjects.Update);

        var setttingDisplay = L(AccountingPermissions.PermissionPrefix + nameof(AccountingPermissions.AccountingSetting));
        context.AddGroup(AccountingPermissions.AccountingSetting, setttingDisplay)
            .AddPermission(AccountingPermissions.AccountingSetting, setttingDisplay);

        AddPermissionGroup(context, AccountingPermissions.TransferVouchers.Default, AccountingPermissions.TransferVouchers.Name,
           AccountingPermissions.TransferVouchers.Create, AccountingPermissions.TransferVouchers.Delete,
            AccountingPermissions.TransferVouchers.Update);
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