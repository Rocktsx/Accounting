using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Accounting.Permissions;
using Accounting.BasicData.Companies;
using Accounting.Common;
using Volo.Abp.Data;

namespace Accounting.BasicData
{
    public class CompanyAppService : AccountingAppService, ICompanyAppService
    {
        private readonly ICompanyRepository _companyRepository;
        protected FunctionCodes FunctionCode { get; set; } = FunctionCodes.Client;
        public CompanyAppService(ICompanyRepository companyRepository)
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
            var service = LazyServiceProvider.LazyGetRequiredService<CodeGenerator>();
            await service.GenerateCodeAsync(company, new CodeCacheItem
            {
                TenantId = CurrentTenant.Id,
                FunctionCode = FunctionCode
            }, getLastNumber: async (prefix) => (int)await _companyRepository.GetLastNumber(prefix));
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
            var item = await _companyRepository.FindWithDetailsAsync(companyId);
            return item ?? throw new EntityNotFoundException();
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
            var list = await _companyRepository.GetPagedListAsync(input.Filter, input.IsClient, input.IsVendor,
                input.Ids, null, false, input.Sorting, input.MaxResultCount, input.SkipCount);
            var count = await _companyRepository.GetCountAsync(input.Filter, input.IsClient, input.IsVendor, input.Ids);
            return new PagedResultDto<CompanyDto>(count, ObjectMapper.Map<IEnumerable<Company>, List<CompanyDto>>(list));
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
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

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
        [RemoteService(false)]
        public async Task<int> ImportDataAsync(IEnumerable<CompanyImportDto> inputs)
        {
            Check.NotNull(inputs, nameof(inputs));

            var companyGroups = inputs.GroupBy(item => item.Code);
            var codes = companyGroups.Select(item => item.Key).Distinct().ToList();
            await ImportHelper.CheckExistsCodesAsync(codes, L, async (codes) =>
                (await _companyRepository.GetPagedListAsync(codes: codes)).Select(item => item.Code));

            var entities = new List<Company>(inputs.Count());
            foreach (var item in companyGroups)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    continue;
                }
                var firstItem = item.FirstOrDefault(obj => !string.IsNullOrWhiteSpace(obj.Name));
                if (firstItem == null)
                {
                    continue;
                }
                var company = new Company(GuidGenerator.Create(), firstItem.Name, firstItem.OtherName,
                    firstItem.NickName, firstItem.Currency, firstItem.CreditLimit ?? 0, firstItem.PaymentTerm,
                    firstItem.TradeTerm, firstItem.IsClient, firstItem.IsVendor, CurrentTenant.Id);

                company.SetCode(firstItem.Code, firstItem.Code, 1);

                foreach (var groupItem in item)
                {
                    if (!string.IsNullOrWhiteSpace(groupItem.Address))
                    {
                        company.AddAddress(GuidGenerator.Create(), groupItem.IsBilling, groupItem.IsShipping, groupItem.AddressName,
                            groupItem.Address, groupItem.ContactPerson, groupItem.Telephone, groupItem.Email, groupItem.Remark,
                            groupItem.Country, groupItem.Region, groupItem.District, groupItem.Fax);
                    }
                    if (!string.IsNullOrEmpty(groupItem.ContactPerson))
                    {
                        company.AddContact(GuidGenerator.Create(), groupItem.ContactPerson, groupItem.Department, groupItem.Position,
                            groupItem.DirectLine, groupItem.Telephone, groupItem.Fax, groupItem.Email, groupItem.Remark);
                    }
                }
                entities.Add(company);
            }

            await _companyRepository.InsertManyAsync(entities);

            return entities.Count;
        }
    }
}
