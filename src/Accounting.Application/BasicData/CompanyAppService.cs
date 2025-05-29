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

        public async Task DeleteAsync(Guid companyId)
        {
            await _companyRepository.DeleteAsync(companyId);
        }

        public async Task<CompanyDto> GetAsync(Guid companyId)
        { 
            var entity = await GetItemWithDetailsAsync(companyId);
            return ObjectMapper.Map<Company, CompanyDto>(entity);
        }
        private async Task<Company> GetItemWithDetailsAsync(Guid companyId)
        {
            var queryable = await _companyRepository.WithDetailsAsync(item => item.Addresses, item => item.Contacts);

            var item = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(item => item.Id == companyId));
            if(item == null)
            {
                throw new EntityNotFoundException();
            }
            return item;
        }

        public async Task<PagedResultDto<CompanyDto>> GetListAsync(CompanySearchDto dto)
        {
            var queryable = await _companyRepository.GetQueryableAsync();
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(dto.Filter), item => item.Name.Contains(dto.Filter) || item.Code.Contains(dto.Filter));
            queryable = queryable.WhereIf(dto.IsClient.HasValue && dto.IsClient == true, item => item.IsClient == true);
            queryable = queryable.WhereIf(dto.IsVendor.HasValue && dto.IsVendor == true, item => item.IsVendor == true);

            var listQuery = queryable.OrderBy(dto.Sorting ?? nameof(Company.Name)).Skip(dto.SkipCount).Take(dto.MaxResultCount);
            var count = await AsyncExecuter.CountAsync(queryable);
            var list = await AsyncExecuter.ToListAsync(listQuery);
            return new PagedResultDto<CompanyDto>(count, ObjectMapper.Map<List<Company>, List<CompanyDto>>(list));
        }

        public async Task<CompanyDto> UpdateAsync(Guid companyId, CompanyCreateOrEditDto input)
        {
            var entity = await GetItemWithDetailsAsync(companyId); ;
            entity.SetCreditLimit(input.CreditLimit)
                .SetCurrency(input.Currency)
                .SetIsClient(input.IsClient)
                .SetIsVendor(input.IsVendor)
                .SetName(input.Name)
                .SetOtherName(input.OtherName)
                .SetNickName(input.NickName)
                .SetPaymentTerm(input.PaymentTerm)
                .SetTradeTerm(input.TradeTerm);

            if (input.Addresses != null)
            {
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
            if (input.Contacts != null)
            {
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
