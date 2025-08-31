using Accounting.Finance.Dtos;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Accounting.Finance
{
    public interface ISubjectAppService : IApplicationService, ICrudAppService<SubjectDto, Guid,
        FilteredPagedAndSortedResultRequestDto, SubjectCreateDto, SubjectUpdateDto>
    {
        Task<IEnumerable<SubjectSimpleDto>> GetSimpleListAsync();
        Task<IEnumerable<SubjectVoucherSimpleDto>> GetVoucherSimpleListAsync();
    } 
}
