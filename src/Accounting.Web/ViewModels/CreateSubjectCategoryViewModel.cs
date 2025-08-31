using Accounting.Finance; 
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Accounting.Web.ViewModels
{
    public class CreateSubjectCategoryViewModel
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
        public Guid? ParentId { get; set; }
        public CreditDebit CreditDebit { get; set; }
        [SelectItems("AccountTypes")]
        public Guid? AccountTypeId { get; set; }
        public bool ShowDetail { get; set; }
        [MaxLength(AccountingCommonConsts.MaxDescriptionLength)]
        [TextArea(Rows = 3)]
        public string? Description { get; set; }
    }
}
