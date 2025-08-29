using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Dtos
{
    public class AccountingPeriodUpdateDto
    {
        [Required]
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        public bool IsCurrentPeriod { get; set; }
    }
}
