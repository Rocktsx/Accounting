using System.Threading.Tasks;
using Accounting.Finance.Dtos;

namespace Accounting.Finance;

public interface IAccountingSettingAppService
{
    Task<AccountingSettingDto> GetAsync();
    Task<AccountingSettingCompanyDto> GetCompanyAsync();
    Task UpdateAsync(AccountingSettingDto dto);
    Task<string> GetNativeCurrencyAsync();
    Task<string> GetAccountReceivableSubjectCodeAsync();
    Task<string> GetAccountPayableSubjectCodeAsync();
    Task<string> GetPurchaseExpensesSubjectCodeAsync();
    Task<string> GetSalesInvoiceSubjectCodeAsync();
    Task<string> GetStatementNoteAsync();
    Task<string> GetStatementReminderAsync();
    Task<string> GetFinalReminderAsync();
    Task<string> GetTransferVoucherDateFormatAsync();
    Task<string> GetReceivableVoucherDateFormatAsync();
    Task<string> GetPayableVoucherDateFormatAsync();
}