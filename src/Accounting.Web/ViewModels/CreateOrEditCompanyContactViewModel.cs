using Accounting.BasicData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Accounting.Web.ViewModels
{
    public class CreateOrEditCompanyContactViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }
        [HiddenInput(DisplayValue = false)]
        public Guid CompanyId { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        [Required]
        public string ContactName { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? Department { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? Position { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        [DataType(DataType.PhoneNumber)]
        public string? DirectLine { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        [DataType(DataType.PhoneNumber)]
        [Required]
        public string Telephone { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string? Fax { get; set; }
        [MaxLength(CompanyConsts.MaxEmailLength)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [MaxLength(CompanyConsts.MaxRemarkLength)]
        [TextArea(Rows = 2)]
        public string? Remark { get; set; }
    }
}
