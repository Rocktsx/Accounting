using System;

namespace Accounting.Finance.Dtos;

public class AccountingSettingDto
{
    public string CompanyName { get; set; }
    public string CompanyOtherName { get; set; }
    public string CompanyAddress { get; set; }
    public string CompanyOtherAddress { get; set; }
    public string CompanyContract { get; set; }
    public string NativeCurrency { get; set; }
    public string AccountReceivableSubjectCode { get; set; }
    public string AccountPayableSubjectCode { get; set; }
    public string PurchaseExpensesSubjectCode { get; set; }
    public string SalesInvoiceSubjectCode { get; set; }
    public string StatementNote { get; set; }
    public string StatementReminder { get; set; }
    public string FinalReminder { get; set; }
}