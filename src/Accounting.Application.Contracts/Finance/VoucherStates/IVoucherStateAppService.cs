using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.VoucherStates
{
    public interface IVoucherStateAppService
    {
        Task<PagedResultDto<VoucherDto>> GetListAsync(VoucherUpdateStatusDto input);
        Task UpdateStatus(VoucherUpdateStatusDto input, VoucherStatus status);
    }
}
