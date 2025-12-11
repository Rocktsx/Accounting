namespace Accounting.Web.Menus;

public class AccountingMenus
{
    private const string Prefix = "Accounting";

    public const string Home = Prefix + ".Home";

    private const string DisplayNamePrefix = "Menu:";
    private const string Dot = ".";

    private const string BasicDataText = "BasicData";
    private const string ClientText = "Client";
    private const string CurrencyText = "Currency";
    private const string VendorText = "Vendor";

    private const string GeneralLedgerText = "GeneralLedger";
    private const string AccountingPeriodText = "AccountingPeriod";
    private const string SubjectCategoryText = "SubjectCategory";
    private const string GeneralAccountText = "GeneralAccount";
    private const string SubjectText = "Subject";
    private const string TransferVoucherText = "TransferVoucher";
    private const string VoucherStateText = "VoucherState";
    private const string GeneralLedgerReportText = "GeneralLedgerReport";
    private const string SingleCurrencyReportText = "SingleCurrencyReport";
    private const string MultipleCurrencyReportText = "MultipleCurrencyReport";
    private const string MultipleCurrencyGroupReportText = "MultipleCurrencyGroupReport";

    private const string ReceivableText = "Receivable";
    private const string ReceivableVoucherText = "ReceivableVoucher";

    private const string PayableText = "Payable";
    private const string PayableVoucherText = "PayableVoucher";

    public const string BasicData = Prefix + Dot + BasicDataText;
    public const string Client = BasicData + Dot + ClientText;
    public const string Currency = BasicData + Dot + CurrencyText;
    public const string Vendor = BasicData + Dot + VendorText;

    public const string GeneralLedger = Prefix + Dot + GeneralLedgerText;
    public const string AccountingPeriod = GeneralLedger + Dot + AccountingPeriodText;
    public const string SubjectCategory = GeneralLedger + Dot + SubjectCategoryText;
    public const string GeneralAccount = GeneralLedger + Dot + GeneralAccountText;
    public const string Subject = GeneralLedger + Dot + SubjectText;
    public const string TransferVoucher = GeneralLedger + Dot + TransferVoucherText;
    public const string VoucherState = GeneralLedger + Dot + VoucherStateText;

    public const string GeneralLedgerReport = GeneralLedger + Dot + GeneralLedgerReportText;
    public const string GeneralLedgerSingleCurrencyReport = GeneralLedgerReport + Dot + SingleCurrencyReportText;
    public const string GeneralLedgerMultipleCurrencyReport = GeneralLedgerReport + Dot + MultipleCurrencyReportText;
    public const string GeneralLedgerMultipleCurrencyGroupReport = GeneralLedgerReport + Dot + MultipleCurrencyGroupReportText;

    public const string Receivable = Prefix + Dot + ReceivableText;
    public const string ReceivableVoucher = Receivable + Dot + ReceivableVoucherText;

    public const string Payable = Prefix + Dot + PayableText;
    public const string PayableVoucher = Payable + Dot + PayableVoucherText;

    public class DisplayNames
    {
        public const string BasicData = DisplayNamePrefix + BasicDataText;
        public const string Client = DisplayNamePrefix + ClientText;
        public const string Currency = DisplayNamePrefix + CurrencyText;
        public const string Vendor = DisplayNamePrefix + VendorText;

        public const string GeneralLedger = DisplayNamePrefix + GeneralLedgerText;
        public const string AccountingPeriod = DisplayNamePrefix + AccountingPeriodText;
        public const string SubjectCategory = DisplayNamePrefix + SubjectCategoryText;
        public const string GeneralAccount = DisplayNamePrefix + GeneralAccountText;
        public const string Subject = DisplayNamePrefix + SubjectText;
        public const string TransferVoucher = DisplayNamePrefix + TransferVoucherText;
        public const string VoucherState = DisplayNamePrefix + VoucherStateText;

        public const string GeneralLedgerReport = DisplayNamePrefix + GeneralLedgerReportText;
        public const string GeneralLedgerSingleCurrencyReport = GeneralLedgerReport + Dot + SingleCurrencyReportText;
        public const string GeneralLedgerMultipleCurrencyReport = GeneralLedgerReport + Dot + MultipleCurrencyReportText;
        public const string GeneralLedgerMultipleCurrencyGroupReport = GeneralLedgerReport + Dot + MultipleCurrencyGroupReportText;

        public const string Receivable = DisplayNamePrefix + ReceivableText;
        public const string ReceivableVoucher = DisplayNamePrefix + ReceivableVoucherText;

        public const string Payable = DisplayNamePrefix + PayableText;
        public const string PayableVoucher = DisplayNamePrefix + PayableVoucherText;
    }

}
