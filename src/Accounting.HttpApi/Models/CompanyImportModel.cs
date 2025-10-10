using Accounting.BasicData.Companies;
using AutoMapper;
using Magicodes.ExporterAndImporter.Core; 

namespace Accounting.Models
{
    [AutoMap(typeof(CompanyImportDto), ReverseMap = true)]
    public class CompanyImportModel
    {
        [ImporterHeader(Name = "Code")]
        public string Code { get; set; }

        [ImporterHeader(Name = "Name")]
        public string Name { get; set; }

        [ImporterHeader(Name = "Other Name")]
        public string? OtherName { get; set; }

        [ImporterHeader(Name = "Nick Name")]
        public string? NickName { get; set; }

        [ImporterHeader(Name = "Currency")]
        public string? Currency { get; set; }

        [ImporterHeader(Name = "Credit Limit")]
        public decimal? CreditLimit { get; set; }

        [ImporterHeader(Name = "Payment Term")]
        public string? PaymentTerm { get; set; }

        [ImporterHeader(Name = "Trade Term")]
        public string? TradeTerm { get; set; }

        [ImporterHeader(Name = "Is Client")]
        [ValueMapping("1",true)] 
        public bool IsClient { get; set; }

        [ImporterHeader(Name = "Is Vendor")]
        [ValueMapping("1", true)] 
        public bool IsVendor { get; set; }

        [ImporterHeader(Name = "Is Billing")]
        [ValueMapping("1", true)] 

        public bool IsBilling { get; set; }

        [ImporterHeader(Name = "Is Shipping")]
        [ValueMapping("1", true)] 
        public bool IsShipping { get; set; }

        [ImporterHeader(Name = "Address Name")]
        public string? AddressName { get; set; }

        [ImporterHeader(Name = "Address")]
        public string? Address { get; set; }

        [ImporterHeader(Name = "Contact Name")]
        public string? ContactPerson { get; set; }

        [ImporterHeader(Name = "Telephone")]
        public string? Telephone { get; set; }

        [ImporterHeader(Name = "Fax")]
        public string? Fax { get; set; }

        [ImporterHeader(Name = "Email")]
        public string? Email { get; set; }

        [ImporterHeader(Name = "Remark")]
        public string? Remark { get; set; }

        [ImporterHeader(Name = "Country")]
        public string? Country { get; set; }

        [ImporterHeader(Name = "Region")]
        public string? Region { get; set; }

        [ImporterHeader(Name = "District")]
        public string? District { get; set; }

        [ImporterHeader(Name = "Department")]
        public string? Department { get; set; }

        [ImporterHeader(Name = "Position")]
        public string? Position { get; set; }

        [ImporterHeader(Name = "Direct Line")]
        public string? DirectLine { get; set; }
    }
}
