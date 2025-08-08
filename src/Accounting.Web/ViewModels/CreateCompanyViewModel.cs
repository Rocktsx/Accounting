using Accounting.BasicData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Web.ViewModels
{
    public class CreateCompanyViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        [Required]
        [MaxLength(AccountingCommonConsts.MaxPrefixLength)]
        public string Prefix { get; set; }
        public int? GenNo { get; set; }
        [Required]
        [MaxLength(CompanyConsts.MaxNameLength)]
        public string Name { get; set; }
        [MaxLength(CompanyConsts.MaxNameLength)]
        public string? OtherName { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? NickName { get; set; }
        [MaxLength(CurrencyConsts.MaxCurrencyLength)]
        public string Currency { get; set; }
        public decimal CreditLimit { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? PaymentTerm { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? TradeTerm { get; set; }
        public bool IsClient { get; set; }
        public bool IsVendor { get; set; }

        public List<CreateOrEditCompanyAddressViewModel> Addresses { get; set; } = new List<CreateOrEditCompanyAddressViewModel>();
        public List<CreateOrEditCompanyContactViewModel> Contacts { get; set; } = new List<CreateOrEditCompanyContactViewModel>();
    }
}
