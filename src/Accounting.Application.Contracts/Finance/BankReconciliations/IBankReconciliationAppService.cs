using System;
using Volo.Abp.Application.Services;

namespace Accounting.Finance.BankReconciliations
{
    public interface IBankReconciliationAppService : IApplicationService, 
        ICrudAppService<BankReconciliationDto, BankReconciliationPagedResultDto, Guid, BankReconciliationPagedRequestDto, 
            BankReconciliationCreateDto, BankReconciliationUpdateDto>
    {
    }
}
