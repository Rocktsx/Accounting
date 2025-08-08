using Accounting.BasicData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Accounting.Web.ViewModels
{
    public class CreateOrEditCompanyAddressViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }
        public bool IsBilling { get; set; }
        public bool IsShipping { get; set; }
        [MaxLength(CompanyConsts.MaxNameLength)]
        public string? Name { get; set; }
        [MaxLength(CompanyAddressConsts.MaxAddressLength)]
        [Required]
        [TextArea(Rows = 3)]
        public string Address { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        [Required]
        public string ContactPerson { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Telephone { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? Fax { get; set; }
        [MaxLength(CompanyConsts.MaxEmailLength)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [MaxLength(CompanyConsts.MaxRemarkLength)]
        [TextArea(Rows =3)]
        public string? Remark { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? Country { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? Region { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? District { get; set; }
    }
}
