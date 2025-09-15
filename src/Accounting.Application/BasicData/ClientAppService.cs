using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Accounting.BasicData.Dtos;

namespace Accounting.BasicData
{
    [RemoteService(true, IsMetadataEnabled =true)]
    public class ClientAppService : CompanyAppService, IClientAppService
    {
        public ClientAppService(IRepository<Company, Guid> companyRepository) : base(companyRepository)
        {
        }
        [Authorize(AccountingPermissions.ClientCreation)]
        public override Task<CompanyDto> CreateAsync(CompanyCreateDto input)
        {
            return base.CreateAsync(input);
        }
        [Authorize(AccountingPermissions.ClientDeletion)]
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }
        [Authorize(AccountingPermissions.Client)]
        public override Task<CompanyDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }
        [Authorize(AccountingPermissions.Client)]
        public override Task<PagedResultDto<CompanyDto>> GetListAsync(CompanySearchDto dto)
        {
            dto.IsClient = true;
            return base.GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.ClientEdit)]
        public override Task<CompanyDto> UpdateAsync(Guid id, CompanyUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }
    }
}
