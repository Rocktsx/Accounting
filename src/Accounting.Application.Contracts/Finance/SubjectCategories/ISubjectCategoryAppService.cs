using Accounting.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.Finance.SubjectCategories
{
    /// <summary>
    /// 总账类别
    /// </summary>
    public interface ISubjectCategoryAppService : IApplicationService, ICrudAppService<SubjectCategoryDto, SubjectCategoryFilteredResultDto, Guid,
        SubjectCategoryFilteredRequestDto, SubjectCategoryCreateDto, SubjectCategoryUpdateDto>
    {
        Task<IEnumerable<SubjectCategorySimpleDto>> GetSimpleListAsync();

        Task<int> ImportDataAsync(IEnumerable<SubjectCategoryImportDto> inputs);
    }
}
