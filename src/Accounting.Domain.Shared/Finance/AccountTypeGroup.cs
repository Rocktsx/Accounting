using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance
{
    /// <summary>
    /// 科目类型组别
    /// </summary>
    public enum AccountTypeGroup
    {
        /// <summary>
        /// 资产
        /// </summary>
        Assets = 1,
        /// <summary>
        /// 负债
        /// </summary>
        Liabilities,
        /// <summary>
        /// 资本
        /// </summary>
        Capital,
        /// <summary>
        /// 收入
        /// </summary>
        Income,
        /// <summary>
        /// 支出
        /// </summary>
        Expenses
    }
}
