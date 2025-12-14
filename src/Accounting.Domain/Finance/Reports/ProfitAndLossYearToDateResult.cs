using System;

namespace Accounting.Finance.Reports
{
    public class ProfitAndLossYearToDateResult: ReportBaseResult
    {
        public int SortOrder { get; set; }
        public int Group { get; set; }
        public Guid? AccountTypeId { get; set; }
        public AccountTypeGroup Category { get; set; }
    }
}
