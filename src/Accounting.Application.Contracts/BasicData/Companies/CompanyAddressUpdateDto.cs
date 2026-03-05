using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BasicData.Companies
{
    public class CompanyAddressUpdateDto
    {
        public Guid Id { get; set; }
        public bool IsBilling { get; set; }
        public bool IsShipping { get; set; }
        [MaxLength(CompanyConsts.MaxNameLength)]
        public string Name { get; set; }
        [Required]
        [MaxLength(CompanyAddressConsts.MaxAddressLength)]
        public string Address { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string ContactPerson { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string Telephone { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string Fax { get; set; }
        [MaxLength(CompanyConsts.MaxEmailLength)]
        public string Email { get; set; }
        [MaxLength(CompanyConsts.MaxRemarkLength)]
        public string Remark { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string Country { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string Region { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string District { get; set; }

        public CompanyAddressUpdateDto()
        {
            Name = string.Empty;
            Address = string.Empty;
            ContactPerson = string.Empty;
            Telephone = string.Empty;
            Fax = string.Empty;
            Email = string.Empty;
            Remark = string.Empty;
            Country = string.Empty;
            Region = string.Empty;
            District = string.Empty;
        }
    }
}
