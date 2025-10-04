using System.Threading.Tasks;
using Accounting.Finance.Settings;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance;

public abstract class AccountingSettingAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IAccountingSettingAppService _accountingSettingAppService;

    public AccountingSettingAppServiceTests()
    {
        _accountingSettingAppService = ServiceProvider.GetRequiredService<IAccountingSettingAppService>();
    }

    private AccountingSettingDto GetAccountingSettingDto(string name, string address)
    {
        return new AccountingSettingDto()
        {
            CompanyName = name,
            CompanyOtherName = name,
            CompanyAddress = address,
            CompanyOtherAddress = address,
            CompanyContact = "Tel: 1234234324 FAX: 123423433",
            NativeCurrency = "RMB",
            AccountPayableSubjectCode = "21",
            AccountReceivableSubjectCode = "32",
            PurchaseExpensesSubjectCode = "41",
            SalesInvoiceSubjectCode = "51",
            StatementNote = "THIS IS DEMO",
            StatementReminder = "THIS IS DEMO SECOND",
            FinalReminder = "THIS IS DEMO FINAL"
        };
    }

    [Fact]
    public async Task Can_Update_Accounting_Settings()
    {
        // Arrange
        var dto = GetAccountingSettingDto("DEMO", "GZ");

        // Act
        await _accountingSettingAppService.UpdateAsync(dto);

        // Assert
        var updateDto = await _accountingSettingAppService.GetAsync();
        updateDto.CompanyName.ShouldBe(dto.CompanyName);
        updateDto.CompanyOtherName.ShouldBe(dto.CompanyOtherName);
        updateDto.CompanyAddress.ShouldBe(dto.CompanyAddress);
        updateDto.CompanyOtherAddress.ShouldBe(dto.CompanyOtherAddress);
        updateDto.CompanyContact.ShouldBe(dto.CompanyContact);
        updateDto.NativeCurrency.ShouldBe(dto.NativeCurrency);
        updateDto.AccountPayableSubjectCode.ShouldBe(dto.AccountPayableSubjectCode);
        updateDto.AccountReceivableSubjectCode.ShouldBe(dto.AccountReceivableSubjectCode);
        updateDto.PurchaseExpensesSubjectCode.ShouldBe(dto.PurchaseExpensesSubjectCode);
        updateDto.SalesInvoiceSubjectCode.ShouldBe(dto.SalesInvoiceSubjectCode);
        updateDto.StatementNote.ShouldBe(dto.StatementNote);
        updateDto.StatementReminder.ShouldBe(dto.StatementReminder);
        updateDto.FinalReminder.ShouldBe(dto.FinalReminder);
    }

    [Fact]
    public async Task Can_Get_Accounting_Settings()
    {
        // Act
        var dto = await _accountingSettingAppService.GetAsync();

        // Assert
        dto.CompanyName.ShouldBe(string.Empty);
        dto.CompanyOtherName.ShouldBe(string.Empty);
        dto.CompanyAddress.ShouldBe(string.Empty);
        dto.CompanyOtherAddress.ShouldBe(string.Empty);
        dto.CompanyContact.ShouldBe(string.Empty);
        dto.NativeCurrency.ShouldBe(string.Empty);
        dto.AccountPayableSubjectCode.ShouldBe(string.Empty);
        dto.AccountReceivableSubjectCode.ShouldBe(string.Empty);
        dto.PurchaseExpensesSubjectCode.ShouldBe(string.Empty);
        dto.SalesInvoiceSubjectCode.ShouldBe(string.Empty);
        dto.StatementNote.ShouldBe(string.Empty);
        dto.StatementReminder.ShouldBe(string.Empty);
        dto.FinalReminder.ShouldBe(string.Empty);
    }

    [Fact]
    public async Task Can_Get_Company_Settings()
    {
        // Arrange
        var dto = GetAccountingSettingDto("Test", "Shen Zheng, China");

        // Act
        await _accountingSettingAppService.UpdateAsync(dto);

        // Assert
        var updateDto = await _accountingSettingAppService.GetCompanyAsync();
        updateDto.CompanyName.ShouldBe(dto.CompanyName);
        updateDto.CompanyOtherName.ShouldBe(dto.CompanyOtherName);
        updateDto.CompanyAddress.ShouldBe(dto.CompanyAddress);
        updateDto.CompanyOtherAddress.ShouldBe(dto.CompanyOtherAddress);
        updateDto.CompanyContact.ShouldBe(dto.CompanyContact);
    }

    [Fact]
    public async Task Can_Get_Settings()
    {
        // Arrange
        var dto = GetAccountingSettingDto("DEMO", "GZ");
        await _accountingSettingAppService.UpdateAsync(dto);

        // Act
        var nativeCurrency = await _accountingSettingAppService.GetNativeCurrencyAsync();
        var accountReceivableSubjectCode = await _accountingSettingAppService.GetAccountReceivableSubjectCodeAsync();
        var accountPayableSubjectCode = await _accountingSettingAppService.GetAccountPayableSubjectCodeAsync();
        var purchaseExpensesSubjectCode = await _accountingSettingAppService.GetPurchaseExpensesSubjectCodeAsync();
        var salesInvoiceSubjectCode = await _accountingSettingAppService.GetSalesInvoiceSubjectCodeAsync();
        var statementNote = await _accountingSettingAppService.GetStatementNoteAsync();
        var statementReminder = await _accountingSettingAppService.GetStatementReminderAsync();
        var finalReminder = await _accountingSettingAppService.GetFinalReminderAsync();

        // Assert 
        nativeCurrency.ShouldBe(dto.NativeCurrency);
        accountPayableSubjectCode.ShouldBe(dto.AccountPayableSubjectCode);
        accountReceivableSubjectCode.ShouldBe(dto.AccountReceivableSubjectCode);
        purchaseExpensesSubjectCode.ShouldBe(dto.PurchaseExpensesSubjectCode);
        salesInvoiceSubjectCode.ShouldBe(dto.SalesInvoiceSubjectCode);
        statementNote.ShouldBe(dto.StatementNote);
        statementReminder.ShouldBe(dto.StatementReminder);
        finalReminder.ShouldBe(dto.FinalReminder);
    }
}