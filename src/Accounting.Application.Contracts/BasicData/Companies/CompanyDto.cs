using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Accounting.BasicData.Companies
{
    public class CompanyDto: AuditedEntityDto<Guid>, IHasConcurrencyStamp
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
        public string ConcurrencyStamp { get; set; }
        public IEnumerable<CompanyAddressDto> Addresses { get; set; } = [];
        public IEnumerable<CompanyContactDto> Contacts { get; set; } = [];

        public CompanyDto()
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
