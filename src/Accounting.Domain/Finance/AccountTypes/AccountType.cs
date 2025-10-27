using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance.AccountTypes
{
    /// <summary>
    /// 会计科目类别
    /// </summary>
    public class AccountType : AuditedEntity<Guid>, IMultiTenant, IHasConcurrencyStamp
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string OtherName { get; private set; }

        public Guid? ParentId { get; private set; }
        /// <summary>
        /// 试算表排序
        /// </summary>
        public int TrialBalanceSort { get; private set; }
        /// <summary>
        /// 损益表排序
        /// </summary>
        public int ProfitAndLossSort { get; private set; }
        /// <summary>
        /// 资产负债表排序
        /// </summary>
        public int BalanceSheetSort { get; private set; }

        /// <summary>
        /// 试算表分组
        /// </summary>
        public int TrialBalanceGroup { get; private set; }
        /// <summary>
        /// 损益表分组
        /// </summary>
        public int ProfitAndLossGroup { get; private set; }
        /// <summary>
        /// 资产负债表分组
        /// </summary>
        public int BalanceSheetGroup { get; private set; }

        public AccountTypeTypes Category { get; private set; }

        public Guid? TenantId { get; private set; }

        public string ConcurrencyStamp { get; set; }
        private AccountType() { }

        public AccountType(
            Guid id,
            string code,
            string name,
            string otherName,
            Guid? parentId,
            int trialBalanceSort,
            int profitAndLossSort,
            int balanceSheetSort,
            int trialBalanceGroup,
            int profitAndLossGroup,
            int balanceSheetGroup,
            Guid? tenantId = null,
            AccountTypeTypes category = 0
        ) : base(id)
        {
            SetCode(code);
            SetName(name);
            SetOtherName(otherName);
            SetParentId(parentId);
            SetTrialBalanceSort(trialBalanceSort);
            SetProfitAndLossSort(profitAndLossSort);
            SetBalanceSheetSort(balanceSheetSort);
            SetTrialBalanceGroup(trialBalanceGroup);
            SetProfitAndLossGroup(profitAndLossGroup);
            SetBalanceSheetGroup(balanceSheetGroup);
            SetCategory(category);
            TenantId = tenantId;
        }
        public AccountType SetCode(string code)
        {
            Code = Check.NotNullOrWhiteSpace(code, nameof(code));
            return this;
        }
        public AccountType SetBalanceSheetGroup(int balanceSheetGroup)
        {
            BalanceSheetGroup = balanceSheetGroup;
            return this;
        }

        public AccountType SetProfitAndLossGroup(int profitAndLossGroup)
        {
            ProfitAndLossGroup = profitAndLossGroup;
            return this;
        }

        public AccountType SetTrialBalanceGroup(int trialBalanceGroup)
        {
            TrialBalanceGroup = trialBalanceGroup;
            return this;
        }

        public AccountType SetBalanceSheetSort(int balanceSheetSort)
        {
            BalanceSheetSort = balanceSheetSort;
            return this;
        }

        public AccountType SetProfitAndLossSort(int profitAndLossSort)
        {
            ProfitAndLossSort = profitAndLossSort;
            return this;
        }

        public AccountType SetTrialBalanceSort(int trialBalanceSort)
        {
            TrialBalanceSort = trialBalanceSort;
            return this;
        }

        public AccountType SetParentId(Guid? parentId)
        {
            ParentId = parentId;
            return this;
        }

        public AccountType SetOtherName(string otherName)
        {
            OtherName = otherName ?? string.Empty;
            return this;
        }

        public AccountType SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
            return this;
        }
        public AccountType SetCategory(AccountTypeTypes category)
        {
            Category = category;
            return this;
        }
    }
}
