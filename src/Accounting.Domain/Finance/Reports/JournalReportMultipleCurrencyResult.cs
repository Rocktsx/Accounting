
namespace Accounting.Finance.Reports
{
    public class JournalReportMultipleCurrencyResult: JournalReportSingleCurrencyResult
    {
        public string CurrencyCode { get; set; }
        public decimal ForeignAmount { get; set; }

        public decimal CurrencyRate { get; set; }
        public JournalReportMultipleCurrencyResult()
        {
            CurrencyCode = string.Empty;
        }
    }
}
