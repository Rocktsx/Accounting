using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.BasicData.Currencies
{
    public interface ICurrencyAppService : IApplicationService
    {
        Task<CurrencyDto> CreateAsync(CurrencyCreateDto input);
        Task DeleteAsync(Guid id);

        Task<CurrencyDto> GetAsync(Guid id);

        Task<PagedResultDto<CurrencyDto>> GetListAsync(PagedAndSortedResultRequestDto input);

        Task UpdateAsync(Guid id, CurrencyUpdateDto input);
        Task<IEnumerable<CurrencyDto>> GetActiveListAsync();
    }
}
