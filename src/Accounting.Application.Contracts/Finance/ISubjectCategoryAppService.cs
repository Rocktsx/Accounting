using Accounting.Finance.Dtos;
using System; 
using Volo.Abp.Application.Services;

namespace Accounting.Finance
{
    public interface ISubjectCategoryAppService : IApplicationService, ICrudAppService<SubjectCategoryDto, Guid,
        FilteredPagedAndSortedResultRequestDto, SubjectCategoryCreateDto, SubjectCategoryUpdateDto>
    {
    }
}
