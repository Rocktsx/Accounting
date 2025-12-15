using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.Reports
{
    public class ProfitAndLoss12MonthsResult: ProfitAndLossYearToDateResult
    {
        public decimal JanuaryNativeAmount { get; set; }
        public decimal FebruaryNativeAmount { get; set; }
        public decimal MarchNativeAmount { get; set; }
        public decimal AprilNativeAmount { get; set; }
        public decimal MayNativeAmount { get; set; }
        public decimal JuneNativeAmount { get; set; }
        public decimal JulyNativeAmount { get; set; }
        public decimal AugustNativeAmount { get; set; }
        public decimal SeptemberNativeAmount { get; set; }
        public decimal OctoberNativeAmount { get; set; }
        public decimal NovemberNativeAmount { get; set; }
        public decimal DecemberNativeAmount { get; set; }
    }
}
