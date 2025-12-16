using System;

namespace Accounting.Finance.BalanceSheetReports
{
    public class BalanceSheetMtdYtdRequestDto: BalanceSheetYearToDateRequestDto
    {
        public DateOnly? StartDate { get; set; }
    }
}
