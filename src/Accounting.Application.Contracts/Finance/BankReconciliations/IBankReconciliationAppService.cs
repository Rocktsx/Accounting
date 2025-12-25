using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Accounting.Finance.BankReconciliations
{
    public interface IBankReconciliationAppService : IApplicationService, 
        ICrudAppService<BankReconciliationDto, BankReconciliationPagedResultDto, Guid, BankReconciliationPagedRequestDto, 
            BankReconciliationCreateDto, BankReconciliationUpdateDto>
    {
        Task AddOrUpdateMany(IEnumerable<BankReconciliationAddOrUpdateDto> items);
    }
}
