
using Accounting.Finance.Reports;

namespace Accounting.Finance.GeneralLedgerReports
{
    public class GeneralLedgerSingleCurrencyReportResultDto : ReportBaseDetailResultDto
    {
        public int SortOrder { get; set; }

        public string DocNo { get; set; }

        public GeneralLedgerSingleCurrencyReportResultDto()
        {
            DocNo = string.Empty;
        }
    }
}
