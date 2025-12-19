using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.Reports
{
    public class AgingSummarySingleCurrencyResultDto
    {
        public string SubSubjectCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyOtherName { get; set; }

        public int OverDays { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal PrepaidDeposit { get; set; }
        public decimal OverdueAmount1 { get; set; }
        public decimal OverdueAmount2 { get; set; }
        public decimal OverdueAmount3 { get; set; }
        public decimal OverdueAmount4 { get; set; }
        public AgingSummarySingleCurrencyResultDto()
        {
            SubSubjectCode = string.Empty;
            CompanyName = string.Empty;
            CompanyOtherName = string.Empty;
        }
    }
}
