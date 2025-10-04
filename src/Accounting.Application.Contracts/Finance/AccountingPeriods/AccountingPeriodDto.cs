using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.AccountingPeriods
{
    public class AccountingPeriodDto: AuditedEntityDto<Guid>
    {
        public string Code { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsCurrentPeriod { get; set; }
    }
}
