using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Accounting.BasicData.Companies
{
    public class CompanyUpdateDto: IHasConcurrencyStamp
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
        public string ConcurrencyStamp { get; set; }
        public List<CompanyAddressUpdateDto> Addresses { get; set; } = [];
        public List<CompanyContactUpdateDto> Contacts { get; set; } = [];
    }
}
