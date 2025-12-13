using System;

namespace Accounting.Finance.Reports
{
    public class ReportBaseResult
    {
        public Guid? SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string SubjectOtherName { get; set; }

        public decimal NativeAmount { get; set; }

        public ReportBaseResult()
        {
            SubjectCode = string.Empty;
            SubjectName = string.Empty;
            SubjectOtherName = string.Empty;
        }
    }
}
