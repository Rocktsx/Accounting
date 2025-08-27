using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Web.ViewModels
{
    public class CreateAccountingPeriodViewModel
    {
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        [Display(Name = "AccPeriod")]
        public string Code { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }
        public bool IsCurrentPeriod { get; set; }
    }
}
