using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.AccountTypes
{
    public interface IAccountTypeRepository: IBasicRepository<AccountType, Guid>
    {
        Task<IEnumerable<AccountType>> GetPagedListAsync(
           string filter = null,
           IEnumerable<Guid?> ids= null,
           IEnumerable<string> codes = null,
           string sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(
            string? filter = null,
            IEnumerable<Guid?> ids = null,
            IEnumerable<string> codes = null,
            CancellationToken cancellationToken = default);
    }
}
