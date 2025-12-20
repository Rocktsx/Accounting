
namespace Accounting.Finance.Reports
{
    public class AgingSummaryMultipleCurrencyResultDto: AgingSummarySingleCurrencyResultDto
    {
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal ForeignOutstandingAmount { get; set; }
        public decimal ForeignPrepaidDeposit { get; set; }
        public decimal ForeignOverdueAmount1 { get; set; }
        public decimal ForeignOverdueAmount2 { get; set; }
        public decimal ForeignOverdueAmount3 { get; set; }
        public decimal ForeignOverdueAmount4 { get; set; }
        public AgingSummaryMultipleCurrencyResultDto()
        {
            CurrencyCode = string.Empty;
        }
    }
}
