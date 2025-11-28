using System;

namespace Accounting.Finance
{
    public class ReportBaseDetailResultDto : ReportBaseResultDto
    {
        public DateOnly VoucherDate { get; set; }
        public string VoucherCode { get; set; }

        public string Description { get; set; }

        public ReportBaseDetailResultDto()
        {
            VoucherCode = string.Empty;
            Description = string.Empty;
        }
    }
}
