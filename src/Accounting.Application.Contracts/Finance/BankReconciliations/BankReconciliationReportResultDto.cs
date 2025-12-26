
namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliationReportResultDto: BankReconciliationUnpresentedReportResultDto
    {
        /// <summary>
        /// 是否已兑现
        /// </summary>
        public bool IsPresented { get; set; }

        public int SortOrder { get; set; }

        public decimal PresentedAmount { get; set; }
    }
}
