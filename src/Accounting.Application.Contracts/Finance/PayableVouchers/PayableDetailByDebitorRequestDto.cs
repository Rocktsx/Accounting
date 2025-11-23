using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.PayableVouchers
{
    public class PayableDetailByDebitorRequestDto: PagedResultRequestDto
    {
        public Guid? DebitorId { get; set; }
    }
}
