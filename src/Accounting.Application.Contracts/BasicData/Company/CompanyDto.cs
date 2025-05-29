using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Accounting.BasicData
{
    public class CompanyDto: GenerateCodeDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } 
        public string OtherName { get; set; } 
        public string NickName { get; set; }
        public string Currency { get; set; }
        public decimal CreditLimit { get; set; } 
        public string PaymentTerm { get; set; } 
        public string TradeTerm { get; set; }
        public bool IsClient { get; set; }
        public bool IsVendor { get; set; }

        public IEnumerable<CompanyAddressDto> Addresses { get; set; } = new List<CompanyAddressDto>();
        public IEnumerable<CompanyContactDto> Contacts { get; set; } = new List<CompanyContactDto>();
    }
}
