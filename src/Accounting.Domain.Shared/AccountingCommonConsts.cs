using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting
{
    public static class AccountingCommonConsts
    {
        public const int AmountPrecision = 20;
        public const int AmountScale = 7;
        public const int MaxCodeLength = 30;
        public const int MaxPrefixLength = 20;
        public const int MaxNameLength = 250;
        public const int MaxDescriptionLength = 1000;
        public const int MaxCommonTextFieldLength = 30;
        public const int AmountRoundScale = 2;

        public const string StandardDateFormat = "yyyy-MM-dd";

        /// <summary>
        /// 截至上年结转余额排序
        /// </summary>
        public const int LastYearBfOrder = 1;

        /// <summary>
        /// 当年结转余额排序
        /// </summary>
        public const int CurrentYearBfOrder = 2;

        /// <summary>
        /// 当前期限排序
        /// </summary>
        public const int CurrentPeriodOrder = 3;

        public const string SystemCodeText = "SYSTEM";

        /// <summary>
        /// 截至上年结转余额文本
        /// </summary>
        public const string LastYearBfText = "LAST YEAR B/F";

        /// <summary>
        /// 当年结转余额文本
        /// </summary>
        public const string CurrentYearBfText = "CURRENT YEAR B/F";

        public const int SystemGenGroupSort = 998;
        public const string SystemGenCodeText = "SYS. GEN.";
        public const string SubjectName = "累计损益";
        public const string SubjectOtherName = "Cumulative Profit (Loss)";
    }
}
