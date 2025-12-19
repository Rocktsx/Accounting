
namespace Accounting.Finance.Reports
{
    public class AgingSummaryMultipleCurrencyResult: AgingSummarySingleCurrencyResult
    {
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }

        public AgingSummaryMultipleCurrencyResult()
        {
            CurrencyCode = string.Empty;
        }
    }
}
