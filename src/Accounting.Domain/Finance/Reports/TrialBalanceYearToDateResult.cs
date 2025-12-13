using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.Reports
{
    public class TrialBalanceYearToDateResult: ReportBaseResult
    {
        public int SortOrder { get; set; }
        public int Group { get; set; }
        public Guid? AccountTypeId { get; set; }
    }
}
