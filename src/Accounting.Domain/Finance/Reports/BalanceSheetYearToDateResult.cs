using System;

namespace Accounting.Finance.Reports
{
    public class BalanceSheetYearToDateResult: ReportBaseResult
    {
        public int SortOrder { get; set; }
        public int Group { get; set; }
        public Guid? AccountTypeId { get; set; }
    }
}
