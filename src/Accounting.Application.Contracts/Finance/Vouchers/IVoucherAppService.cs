using System;
using Volo.Abp.Application.Services;

namespace Accounting.Finance.Vouchers;

public interface IVoucherAppService : ICrudAppService<VoucherDto, Guid,
    VoucherFilterRequestDto, VoucherCreateDto, VoucherUpdateDto>, IApplicationService
{
}