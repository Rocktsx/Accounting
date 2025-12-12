
namespace Accounting.Finance.JournalReports
{
    public class JournalMultipleCurrencyReportResultDto: JournalSingleCurrencyReportResultDto
    {
        public string CurrencyCode { get; set; }
        public decimal ForeignAmount { get; set; }

        public decimal CurrencyRate { get; set; }
        public JournalMultipleCurrencyReportResultDto()
        {
            CurrencyCode = string.Empty;
        }
    }
}
