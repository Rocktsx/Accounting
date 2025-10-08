using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Features
{
    public class AccountingFeatures
    {
        public const string GroupName = "AccountingFeature";

        public const string ProjectFunction = GroupName + ".ProjectFunction";
        public const string RegionFunction = GroupName + ".RegionFunction";
        public const string DepartmentFunction = GroupName + ".DepartmentFunction";
        public const string Custom1Function = GroupName + ".Custom1Function";
        public const string Custom2Function = GroupName + ".Custom2Function";

        /// <summary>
        /// 科目类型管理功能
        /// </summary>
        public const string AccountTypeFunction = GroupName + ".AccountTypeFunction";
    }
}
