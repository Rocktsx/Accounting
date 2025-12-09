using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.Reports
{
    public class ReportGroupBaseResultDto : ReportBaseResultDto
    {
        public int SortOrder { get; set; }
        public int Group { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string GroupOtherName { get; set; }
        public string OtherGroupCode { get; set; }
        public string OtherGroupName { get; set; }
        public string OtherGroupOtherName { get; set; } 

        public ReportGroupBaseResultDto()
        {
            GroupCode = string.Empty;
            GroupName = string.Empty;
            GroupOtherName = string.Empty;
            OtherGroupCode = string.Empty;
            OtherGroupName = string.Empty;
            OtherGroupOtherName = string.Empty;
        }
    }
}
