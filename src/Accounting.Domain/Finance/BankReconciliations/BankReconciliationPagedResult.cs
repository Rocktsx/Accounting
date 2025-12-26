using System;

namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliationPagedResult
    {
        public Guid? Id { get; set; }
        public Guid VoucherDetailId { get; set; }
        /// <summary>
        /// 是否已兑现
        /// </summary>
        public bool IsPresented { get; set; }

        public DateOnly VoucherDate { get; set; }

        public string VoucherCode { get; set; }
        public string Description { get; set; }
        public string PaymentReference { get; set; }

        public decimal NativeAmount { get; set; }
        public DebitorCreditor DebitorCreditor { get; set; }

        public BankReconciliationPagedResult()
        {
            VoucherCode = string.Empty;
            Description = string.Empty;
            PaymentReference = string.Empty;
        }
    }
}
