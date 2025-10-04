using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Accounting.BasicData.Companies
{
    public class CompanyDto: AuditedEntityDto<Guid>
    {
        public string Code { get; set; }
        public string Prefix { get; set; }
        public int GenNo { get; set; }
        public string Name { get; set; } 
        public string OtherName { get; set; } 
        public string NickName { get; set; }
        public string Currency { get; set; }
        public decimal CreditLimit { get; set; } 
        public string PaymentTerm { get; set; } 
        public string TradeTerm { get; set; }
        public bool IsClient { get; set; }
        public bool IsVendor { get; set; }

        public IEnumerable<CompanyAddressDto> Addresses { get; set; } = [];
        public IEnumerable<CompanyContactDto> Contacts { get; set; } = [];
    }
}
