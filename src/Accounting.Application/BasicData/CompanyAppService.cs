using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Accounting.BasicData
{
    public class CompanyAppService : ApplicationService, ICompanyAppService
    {
        private readonly IRepository<Company, Guid> _companyRepository;

        public CompanyAppService(IRepository<Company, Guid> companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public async Task<CompanyDto> CreateAsync(CompanyCreateOrEditDto input)
        {
            var company = new Company(GuidGenerator.Create(), input.Name, input.OtherName,
                input.NickName, input.Currency, input.CreditLimit, input.PaymentTerm, input.TradeTerm,
                input.IsClient, input.IsVendor);
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
            await (new GenerateCodeService()).GenerateCodeAsync(company, _companyRepository);
            var createdCompany = await _companyRepository.InsertAsync(company);
            return ObjectMapper.Map<Company, CompanyDto>(createdCompany);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _companyRepository.DeleteAsync(id);
        }

        public async Task<CompanyDto> GetAsync(Guid id)
        {
            var entity = await GetItemWithDetailsAsync(id);
            return ObjectMapper.Map<Company, CompanyDto>(entity);
        }
        private async Task<Company> GetItemWithDetailsAsync(Guid companyId)
        {
            var queryable = await _companyRepository.WithDetailsAsync(item => item.Addresses, item => item.Contacts);

            var item = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(item => item.Id == companyId));
            if (item == null)
            {
                throw new EntityNotFoundException();
            }
            return item;
        }

        public async Task<PagedResultDto<CompanyDto>> GetListAsync(CompanySearchDto dto)
        {
            var queryable = await _companyRepository.GetQueryableAsync();
            var filter = dto.Filter ?? string.Empty;
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(filter), item => item.Name.Contains(filter) || item.Code.Contains(filter));
            queryable = queryable.WhereIf(dto.IsClient.HasValue && dto.IsClient == true, item => item.IsClient == true);
            queryable = queryable.WhereIf(dto.IsVendor.HasValue && dto.IsVendor == true, item => item.IsVendor == true);

            var listQuery = queryable.OrderBy(dto.Sorting ?? nameof(Company.Name)).Skip(dto.SkipCount).Take(dto.MaxResultCount);
            var count = await AsyncExecuter.CountAsync(queryable);
            var list = await AsyncExecuter.ToListAsync(listQuery);
            return new PagedResultDto<CompanyDto>(count, ObjectMapper.Map<List<Company>, List<CompanyDto>>(list));
        }

        public async Task<CompanyDto> UpdateAsync(Guid id, CompanyCreateOrEditDto input)
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

            if (input.Addresses == null || input.Addresses.Count() == 0)
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
            if (input.Contacts == null || input.Contacts.Count() == 0)
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
