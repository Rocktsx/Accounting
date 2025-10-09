using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BasicData.Companies
{
    public class CompayImportDto
    {
        public string Code { get; set; } 
        public string Name { get; set; }
        public string? OtherName { get; set; }
        public string? NickName { get; set; }
        public string? Currency { get; set; }
        public decimal CreditLimit { get; set; }
        public string? PaymentTerm { get; set; }
        public string? TradeTerm { get; set; }
        public bool IsClient { get; set; }
        public bool IsVendor { get; set; }

        public bool IsBilling { get; set; }
        public bool IsShipping { get; set; }
        public string? AddressName { get; set; }
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public string? Remark { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? District { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? DirectLine { get; set; }
    }
}
