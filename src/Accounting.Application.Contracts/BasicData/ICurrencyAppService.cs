using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Accounting.BasicData;

namespace Accounting.BasicData
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
