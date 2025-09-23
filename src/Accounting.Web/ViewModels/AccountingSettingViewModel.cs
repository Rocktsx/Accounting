using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Accounting.Web.ViewModels;

public class AccountingSettingViewModel
{
    public string? CompanyName { get; set; }
    public string? CompanyOtherName { get; set; }
    [TextArea(Rows = 3)]
    public string? CompanyAddress { get; set; }
    [TextArea(Rows = 3)]
    public string? CompanyOtherAddress { get; set; }
    public string? CompanyContact { get; set; }
    [SelectItems("Currencies")]
    public string? NativeCurrency { get; set; }
    [SelectItems("Subjects")]
    public string? AccountReceivableSubjectCode { get; set; }
    [SelectItems("Subjects")]
    public string? AccountPayableSubjectCode { get; set; }
    [SelectItems("Subjects")]
    public string? PurchaseExpensesSubjectCode { get; set; }
    [SelectItems("Subjects")]
    public string? SalesInvoiceSubjectCode { get; set; }
    [TextArea(Rows = 3)]
    public string? StatementNote { get; set; }
    [TextArea(Rows = 3)]
    public string? StatementReminder { get; set; }
    [TextArea(Rows = 3)]
    public string? FinalReminder { get; set; }
    public string? TransferVoucherDateFormat { get; set; }
    public string? ReceivableVoucherDateFormat { get; set; }

    public string? PayableVoucherDateFormat { get; set; }
}