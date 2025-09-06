using System.Linq;
using System.Threading.Tasks;
using Accounting.Finance.Dtos;
using Accounting.Settings;
using Volo.Abp.SettingManagement;

namespace Accounting.Finance;

public class AccountingSettingAppService: AccountingAppService, IAccountingSettingAppService
{
    private readonly ISettingManager  _settingManager;

    public AccountingSettingAppService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }
    public async Task<AccountingSettingDto> GetAsync()
    {
        var dto = new AccountingSettingDto();
        var settings = (await SettingProvider.GetAllAsync([
            AccountingSettings.CompanyName,
            AccountingSettings.CompanyOtherName,
            AccountingSettings.CompanyAddress,
            AccountingSettings.CompanyOtherAddress,
            AccountingSettings.CompanyContact,
            AccountingSettings.NativeCurrency,
            AccountingSettings.AccountReceivableSubjectCode,
            AccountingSettings.AccountPayableSubjectCode,
            AccountingSettings.PurchaseExpensesSubjectCode,
            AccountingSettings.SalesInvoiceSubjectCode,
            AccountingSettings.StatementNote,
            AccountingSettings.StatementReminder,
            AccountingSettings.FinalReminder
        ])).ToDictionary(item => item.Name, item => item.Value);
        dto.CompanyName = settings[AccountingSettings.CompanyName]??string.Empty;
        dto.CompanyOtherName = settings[AccountingSettings.CompanyOtherName]??string.Empty;
        dto.CompanyAddress = settings[AccountingSettings.CompanyAddress]??string.Empty;
        dto.CompanyOtherAddress = settings[AccountingSettings.CompanyOtherAddress]??string.Empty;
        dto.CompanyContact = settings[AccountingSettings.CompanyContact]??string.Empty;
        dto.NativeCurrency = settings[AccountingSettings.NativeCurrency]??string.Empty;
        dto.AccountReceivableSubjectCode = settings[AccountingSettings.AccountReceivableSubjectCode]??string.Empty;
        dto.AccountPayableSubjectCode = settings[AccountingSettings.AccountPayableSubjectCode]??string.Empty;
        dto.PurchaseExpensesSubjectCode = settings[AccountingSettings.PurchaseExpensesSubjectCode]??string.Empty;
        dto.SalesInvoiceSubjectCode = settings[AccountingSettings.SalesInvoiceSubjectCode]??string.Empty;
        dto.StatementNote = settings[AccountingSettings.StatementNote]??string.Empty;
        dto.StatementReminder = settings[AccountingSettings.StatementReminder]??string.Empty;
        dto.FinalReminder = settings[AccountingSettings.FinalReminder]??string.Empty;
        
        return dto;
    }

    public async Task<AccountingSettingCompanyDto> GetCompanyAsync()
    {
        var dto = new AccountingSettingCompanyDto();
        var settings = (await SettingProvider.GetAllAsync([
            AccountingSettings.CompanyName,
            AccountingSettings.CompanyOtherName,
            AccountingSettings.CompanyAddress,
            AccountingSettings.CompanyOtherAddress,
            AccountingSettings.CompanyContact
        ])).ToDictionary(item => item.Name, item => item.Value);
        dto.CompanyName = settings[AccountingSettings.CompanyName]??string.Empty;
        dto.CompanyOtherName = settings[AccountingSettings.CompanyOtherName]??string.Empty;
        dto.CompanyAddress = settings[AccountingSettings.CompanyAddress]??string.Empty;
        dto.CompanyOtherAddress = settings[AccountingSettings.CompanyOtherAddress]??string.Empty;
        dto.CompanyContact = settings[AccountingSettings.CompanyContact]??string.Empty;
          
        return dto;
    }

    public async Task UpdateAsync(AccountingSettingDto dto)
    {
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.CompanyName, dto.CompanyName);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.CompanyOtherName, dto.CompanyOtherName);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.CompanyAddress, dto.CompanyAddress);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.CompanyOtherAddress, dto.CompanyOtherAddress);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.CompanyContact, dto.CompanyContact);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.NativeCurrency, dto.NativeCurrency);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.AccountPayableSubjectCode, dto.AccountPayableSubjectCode);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.AccountReceivableSubjectCode, dto.AccountReceivableSubjectCode);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.PurchaseExpensesSubjectCode, dto.PurchaseExpensesSubjectCode);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.SalesInvoiceSubjectCode, dto.SalesInvoiceSubjectCode);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.StatementNote, dto.StatementNote);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.StatementReminder, dto.StatementReminder);
         await _settingManager.SetForCurrentTenantAsync(AccountingSettings.FinalReminder, dto.FinalReminder);
    }

    public async Task<string> GetNativeCurrencyAsync()
    {
        return (await SettingProvider.GetOrNullAsync(AccountingSettings.NativeCurrency))??string.Empty;
    }

    public async Task<string> GetAccountReceivableSubjectCodeAsync()
    {
        return (await SettingProvider.GetOrNullAsync(AccountingSettings.AccountReceivableSubjectCode))??string.Empty;
    }

    public async Task<string> GetAccountPayableSubjectCodeAsync()
    {
        return (await SettingProvider.GetOrNullAsync(AccountingSettings.AccountPayableSubjectCode))??string.Empty;
    }

    public async Task<string> GetPurchaseExpensesSubjectCodeAsync()
    {
        return (await SettingProvider.GetOrNullAsync(AccountingSettings.PurchaseExpensesSubjectCode))??string.Empty;
    }

    public async Task<string> GetSalesInvoiceSubjectCodeAsync()
    {
        return (await SettingProvider.GetOrNullAsync(AccountingSettings.SalesInvoiceSubjectCode))??string.Empty;
    }

    public async Task<string> GetStatementNoteAsync()
    {
        return (await SettingProvider.GetOrNullAsync(AccountingSettings.StatementNote))??string.Empty;
    }

    public async Task<string> GetStatementReminderAsync()
    {
        return (await SettingProvider.GetOrNullAsync(AccountingSettings.StatementReminder))??string.Empty;
    }

    public async Task<string> GetFinalReminderAsync()
    {
        return (await SettingProvider.GetOrNullAsync(AccountingSettings.FinalReminder))??string.Empty;
    }
}