using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.BasicData
{
    public interface ICompanyAppService :  IApplicationService
    {
        Task<CompanyDto> GetAsync(Guid id);
        Task<PagedResultDto<CompanyDto>> GetListAsync(CompanySearchDto dto);
        Task<CompanyDto> CreateAsync(CompanyCreateOrEditDto input);
        Task<CompanyDto> UpdateAsync(Guid id,  CompanyCreateOrEditDto input);
        Task DeleteAsync(Guid id);
    } 
}
