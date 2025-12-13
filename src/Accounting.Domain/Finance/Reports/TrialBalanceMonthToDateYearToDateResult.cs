

namespace Accounting.Finance.Reports
{
    public class TrialBalanceMonthToDateYearToDateResult: TrialBalanceYearToDateResult
    {
        public decimal MonthToDateNativeAmount { get; set; }
        public decimal LastPeriodNativeAmount { get; set; }
    }
}
