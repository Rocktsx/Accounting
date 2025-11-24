using Accounting.Finance.ReceivableVouchers;
using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.PayableVouchers
{
    public class GeneratePayableDetailRequestDto
    {
        /// <summary>
        /// 付款明细
        /// </summary>
        public IEnumerable<PaymentDetailDto> Payments { get; set; } = [];
        /// <summary>
        /// 收款明细
        /// </summary>
        public IEnumerable<PayableDetailDto> Receipts { get; set; } = [];

        public Guid? DebitorId { get; set; }
    }
}
