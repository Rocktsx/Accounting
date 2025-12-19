
namespace Accounting.Finance.Reports
{
    public class AgingSummaryMultipleCurrencyResultDto: AgingSummarySingleCurrencyResultDto
    {
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }

        public AgingSummaryMultipleCurrencyResultDto()
        {
            CurrencyCode = string.Empty;
        }
    }
}
