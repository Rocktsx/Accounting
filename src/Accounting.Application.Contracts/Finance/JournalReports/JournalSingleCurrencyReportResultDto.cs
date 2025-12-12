using Accounting.Finance.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.JournalReports
{
    public class JournalSingleCurrencyReportResultDto: ReportBaseDetailResultDto
    {
        public DebitorCreditor DebitorCreditor { get; set; }
    }
}
