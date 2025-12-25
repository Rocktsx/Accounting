using System;

namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliationAddOrUpdateDto
    {
        public Guid? Id { get; set; }
        public Guid? VoucherDetailId { get; set; }
        /// <summary>
        /// 是否已兑现
        /// </summary>
        public bool IsPresented { get; set; }
    }
}
