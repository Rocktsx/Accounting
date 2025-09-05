namespace Accounting.Settings;

public static class AccountingSettings
{
    private const string Prefix = "Accounting";

    //Add your own setting names here. Example:
    //public const string MySetting1 = Prefix + ".MySetting1";
    public const string GroupName = Prefix + ".SettingGroup";
    /// <summary>
    /// 公司名称
    /// </summary>
    public const string CompanyName = Prefix + ".CompanyName";
    /// <summary>
    /// 公司其他名称
    /// </summary>
    public const string CompanyOtherName = Prefix + ".CompanyOtherName";
    /// <summary>
    /// 公司地址
    /// </summary>
    public const string CompanyAddress = Prefix + ".CompanyAddress";
    /// <summary>
    /// 公司其他地址
    /// </summary>
    public const string CompanyOtherAddress = Prefix + ".CompanyOtherAddress";
    /// <summary>
    /// 公司联系方式
    /// </summary>
    public const string CompanyContract = Prefix + ".CompanyContract";

    private const string AccountingPrefix = Prefix + ".Finance";
    /// <summary>
    /// 结算货币
    /// </summary>
    public const string NativeCurrency = AccountingPrefix + ".NativeCurrency";

    /// <summary>
    /// 默认应收科目
    /// </summary>
    public const string AccountReceivableSubjectCode = AccountingPrefix + ".AccountReceivableSubjectCode";

    /// <summary>
    /// 默认应付科目
    /// </summary>
    public const string AccountPayableSubjectCode = AccountingPrefix + ".AccountPayableSubjectCode";

    /// <summary>
    /// 购货科目
    /// </summary>
    public const string PurchaseExpensesSubjectCode = AccountingPrefix + ".PurchaseExpensesSubjectCode";

    /// <summary>
    /// 销售收入科目
    /// </summary>
    public const string SalesInvoiceSubjectCode = AccountingPrefix + ".SalesInvoiceSubjectCode";

    /// <summary>
    /// 月结单说明
    /// </summary>
    public const string StatementNote = AccountingPrefix + ".StatementNote";

    /// <summary>
    /// 第二次通知
    /// </summary>
    public const string StatementReminder = AccountingPrefix + ".StatementReminder";

    /// <summary>
    /// 最后通知
    /// </summary>
    public const string FinalReminder = AccountingPrefix + ".FinalReminder";
}