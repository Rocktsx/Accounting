
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.SubjectCategories
{
    public interface ISubjectCategoryRepository: IBasicRepository<SubjectCategory, Guid>
    {
        Task<IEnumerable<SubjectCategory>> GetPagedListAsync(
           string filter = null,
           IEnumerable<Guid?> ids = null,
           IEnumerable<string> codes = null,
           bool isIncludeAccountType  = false,
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
