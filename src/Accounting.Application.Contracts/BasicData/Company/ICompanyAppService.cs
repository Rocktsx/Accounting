using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Accounting.BasicData.Company
{
    public interface ICompanyAppService :  IApplicationService
    {
        Task<CompanyDto> GetAsync(Guid companyId);
        Task<List<CompanyDto>> GetListAsync(Guid companyId);
        Task<CompanyDto> CreateAsync(CompanyCreateOrEditDto input);
        Task<CompanyDto> UpdateAsync(Guid companyId,  CompanyCreateOrEditDto input);
        Task DeleteAsync(Guid companyId);
    } 
}
