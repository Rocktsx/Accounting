using Accounting.Finance;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Accounting.Web.ViewModels
{
    public class CreateSubjectViewModel
    {
        [Required]
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        [MaxLength(AccountingCommonConsts.MaxNameLength)]
        public string Name { get; set; }
        [MaxLength(AccountingCommonConsts.MaxNameLength)]
        public string? OtherName { get; set; }
        [SelectItems("Categories")]
        [Display(Name = "SubjectCategory")]
        public Guid? SubjectCategoryId { get; set; }
        [SelectItems("AccountTypes")]
        [Display(Name = "AccountType")]
        public Guid? AccountTypeId { get; set; }
        public DebitorCreditor DebitorCreditor { get; set; }
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        [SelectItems("Currencies")]
        [Display(Name = "Currency")]
        public string CurrencyCode { get; set; }
        [MaxLength(AccountingCommonConsts.MaxDescriptionLength)]
        public string? Description { get; set; }
        public bool IsSubSubjectType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPayMethod { get; set; }

        [HiddenInput]
        public int? SeqCode { get; set; }

        public CreateSubjectViewModel()
        {
            Code = string.Empty;
            Name = string.Empty;
            CurrencyCode = string.Empty;
        }
    }
}
