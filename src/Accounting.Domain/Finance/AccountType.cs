using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Accounting.Finance
{
    public class AccountType : Entity<string>
    {
        public string Name { get; private set; }
        public string OtherName { get; private set; }

        public string ParentId { get; private set; }
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

        private AccountType() { }
         
        public AccountType(
            string id,
            string name,
            string otherName,
            string parentId,
            int trialBalanceSort,
            int profitAndLossSort,
            int balanceSheetSort,
            int trialBalanceGroup,
            int profitAndLossGroup,
            int balanceSheetGroup
        ) : base(id)
        {
            SetName(name);
            SetOtherName(otherName);
            SetParentId(parentId);
            SetTrialBalanceSort(trialBalanceSort);
            SetProfitAndLossSort(profitAndLossSort);
            SetBalanceSheetSort(balanceSheetSort);
            SetTrialBalanceGroup(trialBalanceGroup);
            SetProfitAndLossGroup(profitAndLossGroup);
            SetBalanceSheetGroup(balanceSheetGroup);
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

        public AccountType SetParentId(string parentId)
        {
            ParentId = parentId;
            return this;
        }

        public AccountType SetOtherName(string otherName)
        {
            OtherName = otherName;
            return this;
        }

        public AccountType SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
            return this;
        } 
    }
}
