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
    private const string BankReconciliationText = "BankReconciliation";

    private const string GeneralLedgerReportText = "GeneralLedgerReport";
    private const string SingleCurrencyReportText = "SingleCurrencyReport";
    private const string MultipleCurrencyReportText = "MultipleCurrencyReport";
    private const string MultipleCurrencyGroupReportText = "MultipleCurrencyGroupReport";

    private const string JournalReportText = "JournalReport";
    private const string JournalSingleCurrencySortByCodeReportText = "SingleCurrencySortByCodeReport";
    private const string JournalSingleCurrencySortByDateReportText = "SingleCurrencySortByDateReport";
    private const string JournalMultipleCurrencySortByCodeReportText = "MultipleCurrencySortByCodeReport";
    private const string JournalMultipleCurrencySortByDateReportText = "MultipleCurrencySortByDateReport";

    private const string TrialBalanceText = "TrialBalanceReport";
    private const string YearToDateReportText = "YearToDateReport";
    private const string MonthToDateYearToDateReportText = "MonthToDateYearToDateReport";

    private const string ProfitAndLossReportText = "ProfitAndLossReport";
    private const string BalanceSheetReportText = "BalanceSheetReport";

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
    public const string BankReconciliation = GeneralLedger + Dot + BankReconciliationText;

    /// <summary>
    /// General Ledger Report
    /// </summary>
    public class GeneralLedgers
    {
        public const string Name = GeneralLedger + Dot + GeneralLedgerReportText;
        public const string SingleCurrencyReport = Name + Dot + SingleCurrencyReportText;
        public const string MultipleCurrencyReport = Name + Dot + MultipleCurrencyReportText;
        public const string MultipleCurrencyGroupReport = Name + Dot + MultipleCurrencyGroupReportText;
    }

    /// <summary>
    /// Journal Report
    /// </summary>
    public class Journals
    {
        public const string Name = GeneralLedger + Dot + JournalReportText;
        public const string SingleCurrencySortByCodeReport = Name + Dot + JournalSingleCurrencySortByCodeReportText;
        public const string SingleCurrencySortByDateReport = Name + Dot + JournalSingleCurrencySortByDateReportText;
        public const string MultipleCurrencySortByCodeReport = Name + Dot + JournalMultipleCurrencySortByCodeReportText;
        public const string MultipleCurrencySortByDateReport = Name + Dot + JournalMultipleCurrencySortByDateReportText;
    }
    /// <summary>
    /// Trial Balance Report
    /// </summary>
    public class TrialBalances
    {
        public const string Name = GeneralLedger + Dot + TrialBalanceText;
        public const string MonthToDateYearToDateReport = Name + Dot + MonthToDateYearToDateReportText;
        public const string YearToDateReport = Name + Dot + YearToDateReportText;
    }

    private const string TwelveMonthsReportText = "TwelveMonthsReport";
    /// <summary>
    /// Profit And Loss Report
    /// </summary>
    public class ProfitAndLosses
    {
        public const string Name = GeneralLedger + Dot + ProfitAndLossReportText;
        public const string MonthToDateYearToDateReport = Name + Dot + MonthToDateYearToDateReportText;
        public const string YearToDateReport = Name + Dot + YearToDateReportText;
        public const string TwelveMonthsReport = Name + Dot + TwelveMonthsReportText;
    }

    /// <summary>
    /// Balance Sheet Report
    /// </summary>
    public class BalanceSheets
    {
        public const string Name = GeneralLedger + Dot + BalanceSheetReportText;
        public const string MonthToDateYearToDateReport = Name + Dot + MonthToDateYearToDateReportText;
        public const string YearToDateReport = Name + Dot + YearToDateReportText;
    } 

    public const string Receivable = Prefix + Dot + ReceivableText;
    public const string ReceivableVoucher = Receivable + Dot + ReceivableVoucherText;

    private const string ReceivableAgingReportText = "ReceivableAgingReport";
    private const string AgingSummarySingleCurrencyText = "AgingSummarySingleCurrency";
    private const string AgingSummaryMultipleCurrencyText = "AgingSummaryMultipleCurrency";
    private const string AgingDetailText = "AgingDetail";

    public class ReceivableAgingReports {
        public const string Name = Receivable + Dot + ReceivableAgingReportText;
        public const string AgingSummarySingleCurrency = Name + Dot + AgingSummarySingleCurrencyText;
        public const string AgingSummaryMultipleCurrency = Name + Dot + AgingSummaryMultipleCurrencyText;
        public const string AgingDetail = Name + Dot + AgingDetailText;
    }

    public const string Payable = Prefix + Dot + PayableText;
    public const string PayableVoucher = Payable + Dot + PayableVoucherText;

    private const string PayableAgingReportText = "PayableAgingReport";
    public class PayableAgingReports
    {
        public const string Name = Payable + Dot + PayableAgingReportText;
        public const string AgingSummarySingleCurrency = Name + Dot + AgingSummarySingleCurrencyText;
        public const string AgingSummaryMultipleCurrency = Name + Dot + AgingSummaryMultipleCurrencyText;
        public const string AgingDetail = Name + Dot + AgingDetailText;
    }

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
        public const string BankReconciliation = DisplayNamePrefix + BankReconciliationText;

        /// <summary>
        /// General Ledger Report
        /// </summary>
        public class GeneralLedgers
        {
            public const string Name = DisplayNamePrefix + GeneralLedgerReportText;
            public const string SingleCurrencyReport = Name + Dot + SingleCurrencyReportText;
            public const string MultipleCurrencyReport = Name + Dot + MultipleCurrencyReportText;
            public const string MultipleCurrencyGroupReport = Name + Dot + MultipleCurrencyGroupReportText;
        }

        /// <summary>
        /// Journal Report
        /// </summary>
        public class Journals
        {
            public const string Name = DisplayNamePrefix + JournalReportText;
            public const string SingleCurrencySortByCodeReport = Name + Dot + JournalSingleCurrencySortByCodeReportText;
            public const string SingleCurrencySortByDateReport = Name + Dot + JournalSingleCurrencySortByDateReportText;
            public const string MultipleCurrencySortByCodeReport = Name + Dot + JournalMultipleCurrencySortByCodeReportText;
            public const string MultipleCurrencySortByDateReport = Name + Dot + JournalMultipleCurrencySortByDateReportText;
        }

        /// <summary>
        /// Trial Balance Report
        /// </summary>
        public class TrialBalances
        {
            public const string Name = DisplayNamePrefix + TrialBalanceText;
            public const string MonthToDateYearToDateReport = Name + Dot + MonthToDateYearToDateReportText;
            public const string YearToDateReport = Name + Dot + YearToDateReportText;
        }

        /// <summary>
        /// Balance Sheet Report
        /// </summary>
        public class BalanceSheets
        {
            public const string Name = DisplayNamePrefix + BalanceSheetReportText;
            public const string MonthToDateYearToDateReport = Name + Dot + MonthToDateYearToDateReportText;
            public const string YearToDateReport = Name + Dot + YearToDateReportText;
        }

        /// <summary>
        /// Profit And Loss Report
        /// </summary>
        public class ProfitAndLosses
        {
            public const string Name = DisplayNamePrefix + ProfitAndLossReportText;
            public const string MonthToDateYearToDateReport = Name + Dot + MonthToDateYearToDateReportText;
            public const string YearToDateReport = Name + Dot + YearToDateReportText;
            public const string TwelveMonthsReport = Name + Dot + TwelveMonthsReportText;
        }

        public const string Receivable = DisplayNamePrefix + ReceivableText;
        public const string ReceivableVoucher = DisplayNamePrefix + ReceivableVoucherText;

        public class ReceivableAgingReports
        {
            public const string Name = DisplayNamePrefix + ReceivableAgingReportText;
            public const string AgingSummarySingleCurrency = Name + Dot + AgingSummarySingleCurrencyText;
            public const string AgingSummaryMultipleCurrency = Name + Dot + AgingSummaryMultipleCurrencyText;
            public const string AgingDetail = Name + Dot + AgingDetailText;
        }

        public const string Payable = DisplayNamePrefix + PayableText;
        public const string PayableVoucher = DisplayNamePrefix + PayableVoucherText;

        public class PayableAgingReports
        {
            public const string Name = DisplayNamePrefix + PayableAgingReportText;
            public const string AgingSummarySingleCurrency = Name + Dot + AgingSummarySingleCurrencyText;
            public const string AgingSummaryMultipleCurrency = Name + Dot + AgingSummaryMultipleCurrencyText;
            public const string AgingDetail = Name + Dot + AgingDetailText;
        }
    }
}
