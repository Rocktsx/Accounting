using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.ReceivableVouchers
{
    public interface IReceivableVoucherAppService: IVoucherAppService
    {
        Task<PagedResultDto<ReceivableDetailDto>> GetReceivableDetailsByDebitor(
            ReceivableDetailsByDebitorRequestDto input);
    }
}
