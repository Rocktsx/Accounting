using Accounting.BasicData;
using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Web.Pages.BasicData.Currency
{
    public class CreateEditCurrencyViewModel
    {
        [Required]
        [MaxLength(CurrencyConsts.MaxCurrencyLength)]
        public string SourceCurrency { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = CurrencyConsts.AmountFormat)]
        public decimal SourceAmount { get; set; }

        [Required]
        [MaxLength(CurrencyConsts.MaxCurrencyLength)]
        public string TargetCurrency { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = CurrencyConsts.AmountFormat)]
        public decimal TargetAmount { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = CurrencyConsts.AmountFormat)]
        public decimal ExchangeRate { get; set; }
        [DataType(DataType.Date)]
        public DateOnly EffectiveDate { get; set; }
        public bool IsActive { get; set; }

        public CreateEditCurrencyViewModel()
        {
            IsActive = true;
        }
    }
}
