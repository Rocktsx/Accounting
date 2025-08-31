using Accounting.Finance.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Accounting.Finance
{
    public interface ISubjectCategoryAppService : IApplicationService, ICrudAppService<SubjectCategoryDto, Guid,
        FilteredPagedAndSortedResultRequestDto, SubjectCategoryCreateDto, SubjectCategoryUpdateDto>
    {
        Task<IEnumerable<SubjectCategorySimpleDto>> GetSimpleListAsync();
    }
}
