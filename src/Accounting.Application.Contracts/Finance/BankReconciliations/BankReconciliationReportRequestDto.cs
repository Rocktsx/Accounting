using System;

namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliationReportRequestDto
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public Guid? SubjectId { get; set; }
    }
}
