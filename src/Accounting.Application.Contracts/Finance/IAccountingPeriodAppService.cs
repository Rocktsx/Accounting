using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.Finance
{
    public interface IAccountingPeriodAppService : IApplicationService
    {
        Task<AccountingPeriodDto> CreateAsync(AccountingPeriodCreateOrEditDto input);
        Task DeleteAsync(Guid id);

        Task<AccountingPeriodDto> GetAsync(Guid id);

        Task<PagedResultDto<AccountingPeriodDto>> GetListAsync(PagedAndSortedResultRequestDto input);

        Task UpdateAsync(Guid id, AccountingPeriodCreateOrEditDto input);
        Task<IEnumerable<AccountingPeriodDto>> GetCurrentPeriodsAsync();

    }
}
