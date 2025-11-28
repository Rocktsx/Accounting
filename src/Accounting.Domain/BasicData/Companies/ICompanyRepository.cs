using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.BasicData.Companies
{
    public interface ICompanyRepository : IBasicRepository<Company, Guid>
    {
        Task<IEnumerable<Company>> GetPagedListAsync(
            string filter = null,
            bool? isClient = null,
            bool? isVendor = null,
            Guid[]? Ids = null,
            IEnumerable<string>? codes = null,
            bool includeDetails = false,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(
            string? filter = null,
            bool? isClient = null,
            bool? isVendor = null,
            Guid[]? Ids = null,
            IEnumerable<string>? codes = null,
            CancellationToken cancellationToken = default);
        Task<long> GetLastNumber(string prefix, CancellationToken cancellationToken = default);

        Task<Company?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
