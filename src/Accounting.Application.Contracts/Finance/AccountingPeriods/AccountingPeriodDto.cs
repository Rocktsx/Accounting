using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Accounting.Finance.AccountingPeriods
{
    public class AccountingPeriodDto: AuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Code { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsCurrentPeriod { get; set; }

        public string ConcurrencyStamp { get; set; }

        public AccountingPeriodDto()
        {
            Code = string.Empty;
        }
    }
}
