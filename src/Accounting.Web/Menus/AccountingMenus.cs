namespace Accounting.Web.Menus;

public class AccountingMenus
{
    private const string Prefix = "Accounting";

    public const string Home = Prefix + ".Home";

    private const string DisplayNamePrefix = "Menu:";

    public const string BasicData = Prefix + ".BasicData";
    public const string Client = BasicData + ".Client";
    public const string Currency = BasicData + ".Currency";
    public const string Vendor = BasicData + ".Vendor";
    public const string GeneralLedger = Prefix + ".GeneralLedger";
    public const string AccountingPeriod = GeneralLedger + ".AccountingPeriod";
    public const string SubjectCategory = GeneralLedger + ".SubjectCategory";
    public const string GeneralAccount = GeneralLedger + ".GeneralAccount";
    public const string Subject = GeneralLedger + ".Subject";
    public const string TransferVoucher = GeneralLedger + ".TransferVoucher";

    public class DisplayNames
    {
        public const string BasicData = DisplayNamePrefix + "BasicData";
        public const string Client = DisplayNamePrefix + "Client";
        public const string Currency = DisplayNamePrefix + "Currency";
        public const string Vendor = DisplayNamePrefix + "Vendor";
        public const string GeneralLedger = DisplayNamePrefix + "GeneralLedger";
        public const string AccountingPeriod = DisplayNamePrefix + "AccountingPeriod";
        public const string SubjectCategory = DisplayNamePrefix + "SubjectCategory";
        public const string GeneralAccount = DisplayNamePrefix + "GeneralAccount";
        public const string Subject = DisplayNamePrefix + "Subject";
        public const string TransferVoucher = DisplayNamePrefix + "TransferVoucher";
    }

}
