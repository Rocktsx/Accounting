using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.Finance.Subjects
{
    public interface ISubjectAppService : IApplicationService, ICrudAppService<SubjectDto, SubjectFilteredResultDto, Guid,
        SubjectFilterRequestDto, SubjectCreateDto, SubjectUpdateDto>
    {
        Task<IEnumerable<SubjectSimpleDto>> GetSimpleListAsync();
        Task<IEnumerable<SubjectVoucherSimpleDto>> GetVoucherSimpleListAsync();

        Task<int> ImportDataAsync(IEnumerable<SubjectImportDto> inputs);
    } 
}
