using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.ReceivableVouchers
{
    public class ReceivableDetailsByCreditorRequestDto : PagedResultRequestDto
    {
        public Guid? CreditorId { get; set; }
    }
}
