using System;

namespace Accounting.Finance.ProfitAndLossReports
{
    public class ProfitAndLossYearToDateRequestDto
    {
        public Guid? PeriodId { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
