using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Accounting.BasicData.Companies
{
    public interface ICompanyAppService :  IApplicationService
    {
        Task<CompanyDto> GetAsync(Guid id);
        Task<PagedResultDto<CompanyDto>> GetListAsync(CompanySearchDto dto);
        Task<CompanyDto> CreateAsync(CompanyCreateDto input);
        Task<CompanyDto> UpdateAsync(Guid id,  CompanyUpdateDto input);
        Task DeleteAsync(Guid id);
    } 
}
