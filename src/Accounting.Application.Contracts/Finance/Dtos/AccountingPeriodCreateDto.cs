using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Dtos
{
    public class AccountingPeriodCreateDto
    {
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        public bool IsCurrentPeriod { get; set; }
    }
}
