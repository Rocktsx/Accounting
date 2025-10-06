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
    }

}
