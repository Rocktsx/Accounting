using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Accounting.BasicData.Companies;

namespace Accounting.BasicData
{ 
    public class VendorAppService : CompanyAppService, IClientAppService
    {
        public VendorAppService(ICompanyRepository companyRepository) : base(companyRepository)
        {
        }
        [RemoteService(true)]
        [Authorize(AccountingPermissions.Vendors.Create)]
        public override Task<CompanyDto> CreateAsync(CompanyCreateDto input)
        {
            return base.CreateAsync(input);
        }
        [RemoteService(true)]
        [Authorize(AccountingPermissions.Vendors.Delete)]
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }
        [RemoteService(true)]
        [Authorize(AccountingPermissions.Vendors.Default)]
        public override Task<CompanyDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }
        [Authorize(AccountingPermissions.Vendors.Default)]
        public override Task<PagedResultDto<CompanyDto>> GetListAsync(CompanySearchDto dto)
        {
            dto.IsVendor = true;
            return base.QueryListAsync(dto);
        }
        [RemoteService(true)]
        [Authorize(AccountingPermissions.Vendors.Update)]
        public override Task<CompanyDto> UpdateAsync(Guid id, CompanyUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }
    }
}
