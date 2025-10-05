using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Accounting.Finance.Vouchers;

public interface IVoucherAppService : ICrudAppService<VoucherDto, Guid,
    VoucherFilterRequestDto, VoucherCreateDto, VoucherUpdateDto>, IApplicationService
{
    Task UpdateStatus(Guid id, VoucherStatus status);
    Task UpdateManyStatus(VoucherUpdateStatusDto input, VoucherStatus status);
}