using Accounting.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Accounting.Permissions;

public class AccountingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var currencyDisplayName = L(AccountingPermissions.PermissionPrefix + nameof(AccountingPermissions.Currency));
        var currencyGroup = context.AddGroup(AccountingPermissions.Currency, currencyDisplayName); 
        var currency = currencyGroup.AddPermission(AccountingPermissions.Currency, currencyDisplayName);
        currency.AddChild(AccountingPermissions.CurrencyCreation, L(AccountingPermissions.CreationDisplayName));
        currency.AddChild(AccountingPermissions.CurrencyDeletion, L(AccountingPermissions.DeletionDisplayName));
        currency.AddChild(AccountingPermissions.CurrencyEdit, L(AccountingPermissions.EditDisplayName));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountingResource>(name);
    }
}
