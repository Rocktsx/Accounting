using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BasicData.Dtos
{
    public class CompanyAddressDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get;set; }
        public bool IsBilling { get;set; }
        public bool IsShipping { get;set; }
        public string Name { get;set; }
        public string Address { get;set; }
        public string ContactPerson { get;set; }
        public string Telephone { get;set; }
        public string Fax { get;set; }
        public string Email { get;set; }
        public string Remark { get;set; }
        public string Country { get;set; }
        public string Region { get;set; }
        public string District { get;set; }
    }
}
