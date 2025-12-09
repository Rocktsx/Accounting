using System;

namespace Accounting.Finance.Reports
{
    public class ReportBaseResultDto
    {
        public Guid SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string SubjectOtherName { get; set; }

       public decimal NativeAmount { get; set; }

        public ReportBaseResultDto()
        {
            SubjectCode = string.Empty;
            SubjectName = string.Empty;
            SubjectOtherName = string.Empty;
        } 
    }
}
