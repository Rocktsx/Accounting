using Accounting.Finance.Vouchers;
using Accounting.Finance.VoucherStates;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization; 
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.Finance
{
    public class VoucherStateAppService : ApplicationService, IVoucherStateAppService
    {
        private readonly IVoucherAppService _service;

        public VoucherStateAppService(IVoucherAppService service)
        {
            _service = service;
        }
        [Authorize(AccountingPermissions.VoucherStates.Default)]
        public async Task<PagedResultDto<VoucherDto>> GetListAsync(VoucherUpdateStatusDto input)
        {
            return await _service.GetListAsync(new VoucherFilterRequestDto
            {
                VoucherType = input.VoucherType,
                Status = input.Status,
                Filter = input.Code
            });
        }
        [Authorize(AccountingPermissions.VoucherStates.UpdateStatus)]
        public async Task UpdateStatus(VoucherUpdateStatusDto input, VoucherStatus status)
        {
            await _service.UpdateManyStatus(input, status);
        }
    }
}
