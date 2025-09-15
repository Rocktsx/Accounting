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
    [RemoteService(true, IsMetadataEnabled = true)]
    public class VendorAppService : CompanyAppService, IClientAppService
    {
        public VendorAppService(IRepository<Company, Guid> companyRepository) : base(companyRepository)
        {
        }
        [Authorize(AccountingPermissions.VendorCreation)]
        public override Task<CompanyDto> CreateAsync(CompanyCreateDto input)
        {
            return base.CreateAsync(input);
        }
        [Authorize(AccountingPermissions.VendorDeletion)]
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }
        [Authorize(AccountingPermissions.Vendor)]
        public override Task<CompanyDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }
        [Authorize(AccountingPermissions.Vendor)]
        public override Task<PagedResultDto<CompanyDto>> GetListAsync(CompanySearchDto dto)
        {
            dto.IsVendor = true;
            return base.GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.VendorEdit)]
        public override Task<CompanyDto> UpdateAsync(Guid id, CompanyUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }
    }
}
