using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Accounting.BasicData.Dtos;
using Accounting.Permissions;

namespace Accounting.BasicData
{
    public class CompanyAppService : AccountingAppService, ICompanyAppService
    {
        private readonly IRepository<Company, Guid> _companyRepository;

        public CompanyAppService(IRepository<Company, Guid> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [RemoteService(false)]
        public virtual async Task<CompanyDto> CreateAsync(CompanyCreateDto input)
        {
            var company = new Company(GuidGenerator.Create(), input.Name, input.OtherName,
                input.NickName, input.Currency, input.CreditLimit, input.PaymentTerm, input.TradeTerm,
                input.IsClient, input.IsVendor, CurrentTenant.Id);
            company.SetPrefix(input.Prefix);
            if (input.Addresses != null)
            {
                foreach (var address in input.Addresses)
                {
                    company.AddAddress(GuidGenerator.Create(), address.IsBilling, address.IsShipping,
                        address.Name, address.Address, address.ContactPerson, address.Telephone, address.Email,
                        address.Remark, address.Country, address.Region, address.District, address.Fax);
                }
            }
            if (input.Contacts != null)
            {
                foreach (var contact in input.Contacts)
                {
                    company.AddContact(GuidGenerator.Create(), contact.ContactName, contact.Department, contact.Position,
                        contact.DirectLine, contact.Telephone, contact.Fax, contact.Email, contact.Remark);
                }
            }
            var service = LazyServiceProvider.LazyGetRequiredService<GenerateCodeService>();
            await service.GenerateCodeAsync(company, _companyRepository);
            var createdCompany = await _companyRepository.InsertAsync(company);
            return ObjectMapper.Map<Company, CompanyDto>(createdCompany);
        }

        [RemoteService(false)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _companyRepository.DeleteAsync(id);
        }

        [RemoteService(false)]
        public virtual async Task<CompanyDto> GetAsync(Guid id)
        {
            var entity = await GetItemWithDetailsAsync(id);
            return ObjectMapper.Map<Company, CompanyDto>(entity);
        }
        private async Task<Company> GetItemWithDetailsAsync(Guid companyId)
        {
            var queryable = await _companyRepository.WithDetailsAsync(item => item.Addresses, item => item.Contacts);

            var item = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(item => item.Id == companyId));
            return item == null ? throw new EntityNotFoundException() : item;
        }

        public virtual async Task<PagedResultDto<CompanyDto>> GetListAsync(CompanySearchDto dto)
        {
            if (!await CheckPermissions(AccountingPermissions.Clients.Default, AccountingPermissions.Vendors.Default))
            {
                return new PagedResultDto<CompanyDto> { Items = [], TotalCount = 0 };
            }
            return await QueryListAsync(dto);
        }
        protected virtual async Task<PagedResultDto<CompanyDto>> QueryListAsync(CompanySearchDto input)
        {
            var queryable = await _companyRepository.GetQueryableAsync(); 
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), item => item.Name.Contains(input.Filter) 
                || item.Code.Contains(input.Filter) || item.OtherName.Contains(input.Filter) || item.NickName.Contains(input.Filter));
            queryable = queryable.WhereIf(input.IsClient.HasValue && input.IsClient == true, item => item.IsClient == true);
            queryable = queryable.WhereIf(input.IsVendor.HasValue && input.IsVendor == true, item => item.IsVendor == true);
            queryable = queryable.WhereIf(input.Ids != null, item => input.Ids.Contains(item.Id));

            var listQuery = queryable.OrderBy(input.Sorting ?? nameof(Company.Name)).Skip(input.SkipCount).Take(input.MaxResultCount);
            var count = await AsyncExecuter.CountAsync(queryable);
            var list = await AsyncExecuter.ToListAsync(listQuery);
            return new PagedResultDto<CompanyDto>(count, ObjectMapper.Map<List<Company>, List<CompanyDto>>(list));
        }

        [RemoteService(false)]
        public virtual async Task<CompanyDto> UpdateAsync(Guid id, CompanyUpdateDto input)
        {
            var entity = await GetItemWithDetailsAsync(id); ;
            entity.SetCreditLimit(input.CreditLimit)
                .SetCurrency(input.Currency)
                .SetIsClient(input.IsClient)
                .SetIsVendor(input.IsVendor)
                .SetName(input.Name)
                .SetOtherName(input.OtherName)
                .SetNickName(input.NickName)
                .SetPaymentTerm(input.PaymentTerm)
                .SetTradeTerm(input.TradeTerm);

            if (input.Addresses == null || input.Addresses.Count == 0)
            {
                entity.Addresses.Clear();
            }
            else
            {
                entity.Addresses.RemoveAll(address => !input.Addresses.Any(a => a.Id == address.Id));
                foreach (var address in input.Addresses)
                {
                    if (address.Id.Equals(Guid.Empty))
                    {
                        entity.AddAddress(GuidGenerator.Create(), address.IsBilling, address.IsShipping,
                       address.Name, address.Address, address.ContactPerson, address.Telephone, address.Email,
                       address.Remark, address.Country, address.Region, address.District, address.Fax);
                    }
                    else
                    {
                        entity.SetAddress(address.Id, address.IsBilling, address.IsShipping,
                            address.Name, address.Address, address.ContactPerson, address.Telephone, address.Email,
                            address.Remark, address.Country, address.Region, address.District, address.Fax);
                    }
                }
            }
            if (input.Contacts == null || input.Contacts.Count == 0)
            {
                entity.Contacts.Clear();
            }
            else
            {
                entity.Contacts.RemoveAll(contact => !input.Contacts.Any(c => c.Id == contact.Id));
                foreach (var contact in input.Contacts)
                {
                    if (contact.Id.Equals(Guid.Empty))
                    {
                        entity.AddContact(GuidGenerator.Create(), contact.ContactName, contact.Department, contact.Position,
                            contact.DirectLine, contact.Telephone, contact.Fax, contact.Email, contact.Remark);
                    }
                    else
                        entity.SetContact(contact.Id, contact.ContactName, contact.Department, contact.Position,
                       contact.DirectLine, contact.Telephone, contact.Fax, contact.Email, contact.Remark);
                }
            }

            var obj = await _companyRepository.UpdateAsync(entity);
            return ObjectMapper.Map<Company, CompanyDto>(obj);
        }
    }
}
