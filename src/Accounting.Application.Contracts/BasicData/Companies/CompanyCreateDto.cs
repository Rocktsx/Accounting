using Accounting.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BasicData.Companies
{
    public class CompanyCreateDto: GenerateCodeDto
    {
        [Required]
        [MaxLength(CompanyConsts.MaxNameLength)]
        public string Name { get; set; }
        [MaxLength(CompanyConsts.MaxNameLength)]
        public string OtherName { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string NickName { get; set; }
        [MaxLength(CurrencyConsts.MaxCurrencyLength)]
        public string Currency { get; set; }
        public decimal CreditLimit { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string PaymentTerm { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string TradeTerm { get; set; }
        public bool IsClient { get; set; }
        public bool IsVendor { get; set; }

        public List<CompanyAddressCreateDto> Addresses { get; set; } = [];
        public List<CompanyContactCreateDto> Contacts { get; set; } = [];

        public CompanyCreateDto()
        {
            Code = string.Empty;
            Prefix = string.Empty;
            Name = string.Empty;
            OtherName = string.Empty;
            NickName = string.Empty;
            Currency = string.Empty;
            PaymentTerm = string.Empty;
            TradeTerm = string.Empty;
        }
    }
}
