using System;

namespace Accounting.Finance.Reports
{
    public class AgingSummarySingleCurrencyRequestDto
    {
        public Guid? SubSubjectCode { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? AgingDays { get; set; }
    }
}
