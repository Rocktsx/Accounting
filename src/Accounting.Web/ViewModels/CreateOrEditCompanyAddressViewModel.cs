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
        [HiddenInput(DisplayValue = false)]
        public Guid CompanyId { get; set; }
        public bool IsBilling { get; set; }
        public bool IsShipping { get; set; }
        [MaxLength(CompanyConsts.MaxNameLength)]
        public string? Name { get; set; }
        [Required]
        [MaxLength(CompanyAddressConsts.MaxAddressLength)]
        [TextArea(Rows = 3)]
        public required string Address { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        [Required]
        public required string ContactPerson { get; set; }
        [Required]
        [MaxLength(CompanyConsts.CommonMaxLength)]
        [DataType(DataType.PhoneNumber)]
        public required string Telephone { get; set; }
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
