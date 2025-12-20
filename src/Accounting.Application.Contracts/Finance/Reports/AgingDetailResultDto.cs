using System;

namespace Accounting.Finance.Reports
{
    public class AgingDetailResultDto
    {
        public string SubSubjectCode { get; set; }
        public DateOnly VoucherDate { get; set; }

        public string CompanyName { get; set; }
        public string CompanyOtherName { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal PrepaidDeposit { get; set; }
        public decimal ForeignAmount { get; set; }
        public string DocNo { get; set; }
        public AgingDetailResultDto()
        {
            SubSubjectCode = string.Empty;
            CompanyName = string.Empty;
            CompanyOtherName = string.Empty;
            CurrencyCode = string.Empty;
            DocNo = string.Empty;
        }
    }
}
