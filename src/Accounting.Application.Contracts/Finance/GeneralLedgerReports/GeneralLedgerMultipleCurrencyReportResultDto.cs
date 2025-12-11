
namespace Accounting.Finance.GeneralLedgerReports
{
    public class GeneralLedgerMultipleCurrencyReportResultDto : GeneralLedgerSingleCurrencyReportResultDto
    {
        public string CurrencyCode { get; set; }
        public decimal ForeignAmount { get; set; }
        public GeneralLedgerMultipleCurrencyReportResultDto()
        {
            CurrencyCode = string.Empty;
        }
    }
}
