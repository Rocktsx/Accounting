using System;

namespace Accounting.Finance.ProfitAndLossReports
{
    public class ProfitAndLossMtdYtdRequestDto: ProfitAndLossYearToDateRequestDto
    {
        public DateOnly? StartDate { get; set; }
    }
}
