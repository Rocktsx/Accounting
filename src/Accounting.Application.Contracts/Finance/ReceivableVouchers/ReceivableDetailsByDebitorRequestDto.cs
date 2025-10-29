using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.ReceivableVouchers
{
    public class ReceivableDetailsByDebitorRequestDto: PagedResultRequestDto
    {
        public Guid? DebitorId { get; set; }
    }
}
