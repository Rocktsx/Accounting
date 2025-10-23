using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Accounting.Finance.AccountingPeriods
{
    public class AccountingPeriodUpdateDto: IHasConcurrencyStamp
    {
        [Required]
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        public bool IsCurrentPeriod { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}
