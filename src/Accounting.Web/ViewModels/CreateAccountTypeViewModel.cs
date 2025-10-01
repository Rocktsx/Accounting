using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Accounting.Web.ViewModels
{
    public class CreateAccountTypeViewModel
    {
        [Required]
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        [MaxLength(AccountingCommonConsts.MaxNameLength)]
        public string Name { get; set; }
        [MaxLength(AccountingCommonConsts.MaxNameLength)]
        public string OtherName { get; set; }

        [SelectItems("AccountTypes")]
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 试算表排序
        /// </summary>
        public int TrialBalanceSort { get; set; }
        /// <summary>
        /// 损益表排序
        /// </summary>
        public int ProfitAndLossSort { get; set; }
        /// <summary>
        /// 资产负债表排序
        /// </summary>
        public int BalanceSheetSort { get; set; }

        /// <summary>
        /// 试算表分组
        /// </summary>
        public int TrialBalanceGroup { get; set; }
        /// <summary>
        /// 损益表分组
        /// </summary>
        public int ProfitAndLossGroup { get; set; }
        /// <summary>
        /// 资产负债表分组
        /// </summary>
        public int BalanceSheetGroup { get; set; }
    }
}
