using System;
using Accounting.Finance.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.Finance;

public interface IVoucherAppService : ICrudAppService<VoucherDto, Guid,
    VoucherFilterRequestDto, VoucherCreateDto, VoucherUpdateDto>, IApplicationService
{
}