using System;

namespace Accounting.Finance.Settings;

public class AccountingSettingDto
{
    public string CompanyName { get; set; }
    public string CompanyOtherName { get; set; }
    public string CompanyAddress { get; set; }
    public string CompanyOtherAddress { get; set; }
    public string CompanyContact { get; set; }
    public string NativeCurrency { get; set; }
    public string AccountReceivableSubjectCode { get; set; }
    public string AccountPayableSubjectCode { get; set; }
    public string PurchaseExpensesSubjectCode { get; set; }
    public string SalesInvoiceSubjectCode { get; set; }
    public string StatementNote { get; set; }
    public string StatementReminder { get; set; }
    public string FinalReminder { get; set; }
    public string TransferVoucherDateFormat { get; set; }
    public string ReceivableVoucherDateFormat { get; set; }

    public string PayableVoucherDateFormat { get; set; }

    public AccountingSettingDto()
    {
        CompanyName = string.Empty;
        CompanyOtherName = string.Empty;
        CompanyAddress = string.Empty;
        CompanyOtherAddress = string.Empty;
        CompanyContact = string.Empty;
        NativeCurrency = string.Empty;
        AccountReceivableSubjectCode = string.Empty;
        AccountPayableSubjectCode = string.Empty;
        PurchaseExpensesSubjectCode = string.Empty;
        SalesInvoiceSubjectCode = string.Empty;
        StatementNote = string.Empty;
        StatementReminder = string.Empty;
        FinalReminder = string.Empty;
        TransferVoucherDateFormat = string.Empty;
        ReceivableVoucherDateFormat = string.Empty;
        PayableVoucherDateFormat = string.Empty;
    }
}