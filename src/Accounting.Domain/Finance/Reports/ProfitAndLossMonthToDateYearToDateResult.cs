using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.Reports
{
    public class ProfitAndLossMonthToDateYearToDateResult: ProfitAndLossYearToDateResult
    {
        public decimal MonthToDateNativeAmount { get; set; }
        public decimal LastPeriodNativeAmount { get; set; }
    }
}
