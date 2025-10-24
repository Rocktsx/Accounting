using Accounting.Dtos;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Accounting.Finance.AccountTypes
{
    public class AccountTypeDto : AuditedEntityDto<Guid>, IHasConcurrencyStamp
    { 
        public string Code { get; set; } 
        public string Name { get; set; } 
        public string OtherName { get; set; }

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

        public string ConcurrencyStamp { get; set; }

        public AccountTypeTypes Category { get; set; }

        public AccountTypeSimpleDto? Parent { get; set; }
    }
}
