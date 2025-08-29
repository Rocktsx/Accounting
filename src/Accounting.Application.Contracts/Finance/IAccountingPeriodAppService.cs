using Accounting.Finance.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.Finance
{
    public interface IAccountingPeriodAppService : IApplicationService
    {
        Task<AccountingPeriodDto> CreateAsync(AccountingPeriodCreateDto input);
        Task DeleteAsync(Guid id);

        Task<AccountingPeriodDto> GetAsync(Guid id);

        Task<PagedResultDto<AccountingPeriodDto>> GetListAsync(FilteredPagedAndSortedResultRequestDto input);

        Task UpdateAsync(Guid id, AccountingPeriodUpdateDto input);
        Task<CurrentAccountingPeriodDto> GetCurrentPeriodAsync();

    }
}
