using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;

namespace Accounting.Finance.ReceivableVouchers
{
    public class GenerateReceivableDetailRequestDto
    {
        /// <summary>
        /// 付款明细
        /// </summary>
        public IEnumerable<PaymentDetailDto> Payments { get; set; } = [];
        /// <summary>
        /// 收款明细
        /// </summary>
        public IEnumerable<ReceivableDetailDto> Receipts { get; set; } = [];

        public Guid? Creditor { get; set; }
    }
}
