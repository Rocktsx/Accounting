using System;

namespace Accounting.Finance.Reports
{
    public class JournalReportRequest
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public VoucherType? VoucherType { get; set; }
        public string? Prefix { get; set; }
        public int? StartNo { get; set; }
        public int? EndNo { get; set; }
    }
}
