using System;
using System.Collections.Generic; 
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.Subjects
{
    public interface ISubjectRepository: IBasicRepository<Subject, Guid>
    {
        Task<IEnumerable<Subject>> GetPagedListAsync(
           SubjectFilterRequest request = null,
           string sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(SubjectFilterRequest request = null,
            CancellationToken cancellationToken = default);
    }
}
