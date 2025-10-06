using Accounting.BasicData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Web.Pages.BasicData.Currencies
{
    public class CreateEditCurrencyViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
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
        public DateOnly? EffectiveDate { get; set; }
        public bool IsActive { get; set; }

        public CreateEditCurrencyViewModel()
        {
            IsActive = true;
        }
    }
}
