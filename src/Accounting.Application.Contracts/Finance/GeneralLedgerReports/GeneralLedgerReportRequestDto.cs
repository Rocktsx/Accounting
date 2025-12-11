using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.GeneralLedgerReports
{
    public class GeneralLedgerReportRequestDto
    {
        public Guid? SubjectId { get; set; }

        public Guid? PeriodId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
