using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliationCreateDto
    {
        [Required]
        public Guid VoucherDetailId { get; set; }

        /// <summary>
        /// 是否已兑现
        /// </summary>
        public bool IsPresented { get; set; }
    }
}
