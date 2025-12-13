using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.TrialBalanceReports
{
    /// <summary>
    /// Year To Date Request Dto
    /// </summary>
    public class TrialBalanceYtdRequestDto
    {
        public Guid? PeriodId { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
