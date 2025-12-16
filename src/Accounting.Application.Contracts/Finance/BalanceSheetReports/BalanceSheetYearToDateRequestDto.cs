using System;

namespace Accounting.Finance.BalanceSheetReports
{
    public class BalanceSheetYearToDateRequestDto
    {
        public Guid? PeriodId { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
