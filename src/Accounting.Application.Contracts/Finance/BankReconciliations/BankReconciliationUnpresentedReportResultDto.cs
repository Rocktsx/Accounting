using Accounting.Finance.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliationUnpresentedReportResultDto: ReportBaseResultDto
    {
        public DateOnly VoucherDate { get; set; }

        public string VoucherCode { get; set; }
        public string Description { get; set; }

        public DebitorCreditor DebitorCreditor { get; set; }
        public BankReconciliationUnpresentedReportResultDto()
        {
            VoucherCode = string.Empty;
            Description = string.Empty;
        }
    }
}
