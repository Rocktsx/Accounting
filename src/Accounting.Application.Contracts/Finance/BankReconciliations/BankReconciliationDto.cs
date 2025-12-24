using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliationDto : AuditedEntityDto<Guid>
    {
        public Guid VoucherDetailId { get; set; }

        /// <summary>
        /// 是否已兑现
        /// </summary>
        public bool IsPresented { get; set; }
    }
}
