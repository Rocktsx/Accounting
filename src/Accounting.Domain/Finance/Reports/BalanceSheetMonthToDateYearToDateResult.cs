

namespace Accounting.Finance.Reports
{
    public class BalanceSheetMonthToDateYearToDateResult : BalanceSheetYearToDateResult
    {
        public decimal MonthToDateNativeAmount { get; set; }
        public decimal LastPeriodNativeAmount { get; set; }
    }
}
