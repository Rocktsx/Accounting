using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.Reports
{
    /// <summary>
    /// General Ledger Single Currency Report Result
    /// </summary>
    public class GeneralLedgerSingleCurrencyReportResult : ReportBaseDetailResult
    {
        public int SortOrder { get; set; }

        public string DocNo { get; set; }

        public GeneralLedgerSingleCurrencyReportResult()
        {
            DocNo = string.Empty;
        }
    }
}
