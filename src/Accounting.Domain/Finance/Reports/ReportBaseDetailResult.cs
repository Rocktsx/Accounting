using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.Reports
{
    public class ReportBaseDetailResult : ReportBaseResult
    {
        public DateOnly VoucherDate { get; set; }
        public string VoucherCode { get; set; }

        public string Description { get; set; }

        public ReportBaseDetailResult()
        {
            VoucherCode = string.Empty;
            Description = string.Empty;
        }
    }
}
