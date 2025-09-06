using Volo.Abp.Settings;
using Volo.Abp.Localization;
using Accounting.Localization;

namespace Accounting.Settings;

public class AccountingSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here: 
        context.Add(new SettingDefinition(AccountingSettings.CompanyName, string.Empty,
            L(nameof(AccountingSettings.CompanyName))));
        context.Add(new SettingDefinition(AccountingSettings.CompanyOtherName, string.Empty,
            L(nameof(AccountingSettings.CompanyOtherName))));
        context.Add(new SettingDefinition(AccountingSettings.CompanyAddress, string.Empty,
            L(nameof(AccountingSettings.CompanyAddress))));
        context.Add(new SettingDefinition(AccountingSettings.CompanyOtherAddress, string.Empty,
            L(nameof(AccountingSettings.CompanyOtherAddress))));
        context.Add(new SettingDefinition(AccountingSettings.CompanyContact, string.Empty,
            L(nameof(AccountingSettings.CompanyContact))));
        context.Add(new SettingDefinition(AccountingSettings.NativeCurrency, string.Empty,
            L(nameof(AccountingSettings.NativeCurrency))));
        context.Add(new SettingDefinition(AccountingSettings.AccountReceivableSubjectCode, string.Empty,
            L(nameof(AccountingSettings.AccountReceivableSubjectCode))));
        context.Add(new SettingDefinition(AccountingSettings.AccountPayableSubjectCode, string.Empty,
            L(nameof(AccountingSettings.AccountPayableSubjectCode))));
        context.Add(new SettingDefinition(AccountingSettings.PurchaseExpensesSubjectCode, string.Empty,
            L(nameof(AccountingSettings.PurchaseExpensesSubjectCode))));
        context.Add(new SettingDefinition(AccountingSettings.SalesInvoiceSubjectCode, string.Empty,
            L(nameof(AccountingSettings.SalesInvoiceSubjectCode))));
        context.Add(new SettingDefinition(AccountingSettings.StatementNote, string.Empty,
            L(nameof(AccountingSettings.StatementNote))));
        context.Add(new SettingDefinition(AccountingSettings.StatementReminder, string.Empty,
            L(nameof(AccountingSettings.StatementReminder))));
        context.Add(new SettingDefinition(AccountingSettings.FinalReminder, string.Empty,
            L(nameof(AccountingSettings.FinalReminder))));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountingResource>(name);
    }
}