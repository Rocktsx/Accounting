using System;

namespace Accounting.Finance.TrialBalanceReports
{
    /// <summary>
    ///  Month To Date and Year To Date Request Dto
    /// </summary>
    public class TrialBalanceMtdYtdRequestDto: TrialBalanceYtdRequestDto
    {
        public DateOnly? StartDate { get; set; }
    }
}
