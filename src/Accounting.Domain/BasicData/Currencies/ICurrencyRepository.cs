using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.BasicData.Currencies
{
    public interface ICurrencyRepository: IBasicRepository<Currency, Guid>
    {
        Task<IEnumerable<Currency>> GetPagedListAsync(
            string filter = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default); 
        Task<long> GetCountAsync(
            string? filter = null, 
            bool? isActive = null, 
            CancellationToken cancellationToken = default);
    }
}
