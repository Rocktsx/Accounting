using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;
using Accounting.BasicData.Dtos;

namespace Accounting.BasicData
{
    public abstract class CompanyAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ICompanyAppService _companyAppService;

        public CompanyAppServiceTests()
        {
            _companyAppService = GetRequiredService<ICompanyAppService>();
        }
        private static CompanyCreateDto GetCompanyCreateDto()
        {
            var input = new CompanyCreateDto
            {
                Name = "Test Company",
                OtherName = "Test Co.",
                NickName = "TC",
                Currency = "USD",
                CreditLimit = 10000,
                PaymentTerm = "Net 30",
                TradeTerm = "FOB",
                IsClient = true,
                IsVendor = false,
                Prefix = "C"
            };
            AddDetails(input, string.Empty);

            return input;
        }
        private static CompanyUpdateDto GetCompanyUpdateDto()
        {
            var input = new CompanyUpdateDto
            {
                Name = "Test Company",
                OtherName = "Test Co.",
                NickName = "TC",
                Currency = "USD",
                CreditLimit = 10000,
                PaymentTerm = "Net 30",
                TradeTerm = "FOB",
                IsClient = true,
                IsVendor = false,
            };
            AddDetails(input, string.Empty);

            return input;
        }
        private static void AddDetails(CompanyCreateDto input, string suf)
        {
            input.Addresses.Add(new CompanyAddressCreateDto
            {
                IsBilling = true,
                IsShipping = false,
                Name = "Main Office" + suf,
                Address = "123 Main St." + suf,
                ContactPerson = "John Doe" + suf,
                Telephone = "123-456-7890"
            });
            input.Contacts.Add(new CompanyContactCreateDto
            {
                ContactName = "Jane Smith" + suf,
                Department = "Sales" + suf,
                Position = "Manager" + suf,
                DirectLine = "123-456-7891",
                Telephone = "123-456-7892",
                Fax = "123-456-7893"
            });
        }
        private static void AddDetails(CompanyUpdateDto input, string suf)
        {
            input.Addresses.Add(new CompanyAddressUpdateDto
            {
                IsBilling = true,
                IsShipping = false,
                Name = "Main Office" + suf,
                Address = "123 Main St." + suf,
                ContactPerson = "John Doe" + suf,
                Telephone = "123-456-7890"
            });
            input.Contacts.Add(new CompanyContactUpdateDto
            {
                ContactName = "Jane Smith" + suf,
                Department = "Sales" + suf,
                Position = "Manager" + suf,
                DirectLine = "123-456-7891",
                Telephone = "123-456-7892",
                Fax = "123-456-7893"
            });
        }
        [Fact]
        public async Task Should_Create_Company()
        {
            // Arrange
            var input = GetCompanyCreateDto();
            // Act
            var entity = await _companyAppService.CreateAsync(input);
            // Assert
            var target = await _companyAppService.GetAsync(entity.Id);
            target.ShouldNotBeNull();
            target.Id.ShouldBe(entity.Id);
            target.Name.ShouldBe(input.Name);
            target.OtherName.ShouldBe(input.OtherName);
            target.NickName.ShouldBe(input.NickName);
            target.Addresses.ShouldNotBeNull();
            target.Addresses.Count().ShouldBe(1);
            target.Contacts.ShouldNotBeNull();
            target.Contacts.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Should_Update_Company()
        {
            // Arrange
            var input = GetCompanyCreateDto();
            var entity = await _companyAppService.CreateAsync(input);
            var updateInput = GetCompanyUpdateDto();
            updateInput.Name = "Rock Company";
            updateInput.NickName = "Rock";
            updateInput.OtherName = "othRock";
            var address = updateInput.Addresses.First();
            address.Id = entity.Addresses.First().Id;
            address.Name = "Home";
            address.Address = "east way big street, sz";
            var contact = updateInput.Contacts.First();
            contact.Id = entity.Contacts.First().Id;
            contact.ContactName = "Rock";
            contact.Position = "CTO";
            contact.Telephone = "135222244444";
            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _companyAppService.UpdateAsync(entity.Id, updateInput);
            });
            // Assert
            var target = await _companyAppService.GetAsync(entity.Id);
            target.Name.ShouldBe(updateInput.Name);
            target.OtherName.ShouldBe(updateInput.OtherName);
            target.NickName.ShouldBe(updateInput.NickName);
            var targetAddress = target.Addresses.First();
            targetAddress.Name.ShouldBe(address.Name);
            targetAddress.Address.ShouldBe(address.Address);
            var targetContact = target.Contacts.First();
            targetContact.ContactName.ShouldBe(contact.ContactName);
            targetContact.Position.ShouldBe(contact.Position);
            targetContact.Telephone.ShouldBe(contact.Telephone);
        }
        [Fact]
        public async Task Should_Add_Address_Contact_When_Update_Company()
        {
            // Arrange
            var input = GetCompanyCreateDto();
            var entity = await _companyAppService.CreateAsync(input);
            var updateInput = GetCompanyUpdateDto();
            var address = updateInput.Addresses.First();
            address.Id = entity.Addresses.First().Id;
            var contact = updateInput.Contacts.First();
            contact.Id = entity.Contacts.First().Id;
            var suf = "22";
            AddDetails(updateInput, suf);
            // Act
            await _companyAppService.UpdateAsync(entity.Id, updateInput);
            // Assert
            var target = await _companyAppService.GetAsync(entity.Id);
            target.Addresses.Count().ShouldBe(2);
            target.Addresses.ShouldContain(item => item.Name == address.Name + suf);
            target.Addresses.ShouldContain(item => item.Address == address.Address + suf);
            target.Contacts.Count().ShouldBe(2);
            target.Contacts.ShouldContain(item => item.ContactName == contact.ContactName + suf);
            target.Contacts.ShouldContain(item => item.Department == contact.Department + suf);
            target.Contacts.ShouldContain(item => item.Position == contact.Position + suf);
        }
        [Fact]
        public async Task Should_Get_Company()
        {
            // Arrange
            var input = GetCompanyCreateDto();
            var entity = await _companyAppService.CreateAsync(input);
            // Act 
            var target = await _companyAppService.GetAsync(entity.Id);
            // Assert
            target.ShouldNotBeNull();
            target.Id.ShouldBe(entity.Id);
            target.Addresses.ShouldNotBeNull();
            target.Addresses.Count().ShouldBe(1);
            target.Contacts.ShouldNotBeNull();
            target.Contacts.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Should_Delete_Company()
        {
            // Arrange
            var input = GetCompanyCreateDto();
            var entity = await _companyAppService.CreateAsync(input);
            // Act 
            await _companyAppService.DeleteAsync(entity.Id);
            // Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _companyAppService.GetAsync(entity.Id);
            });
            exception.ShouldNotBeNull();
        }
        [Fact]
        public async Task Should_Get_Companies()
        {
            // arrange
            var dto = new CompanySearchDto();
            //act
            var result = await _companyAppService.GetListAsync(dto);

            // assert
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
        }
        [Fact]
        public async Task Should_Get_Client_Companies()
        {
            // arrange
            var input = GetCompanyCreateDto();
            input.IsClient = false;
            await _companyAppService.CreateAsync(input);
            input = GetCompanyCreateDto();
            input.IsClient = false;
            await _companyAppService.CreateAsync(input);
            var dto = new CompanySearchDto()
            {
                IsClient = true
            };
            //act
            var result = await _companyAppService.GetListAsync(dto);

            // assert
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
        }
        [Fact]
        public async Task Should_Get_Vendor_Companies()
        {
            // arrange
            var input = GetCompanyCreateDto();
            input.IsVendor = true;
            input.IsClient = false;
            await _companyAppService.CreateAsync(input);
            input = GetCompanyCreateDto();
            input.IsVendor = true;
            input.IsClient = false;
            await _companyAppService.CreateAsync(input);
            var dto = new CompanySearchDto()
            {
                IsVendor = true
            };
            //act
            var result = await _companyAppService.GetListAsync(dto);

            // assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
        }
        [Fact]
        public async Task Should_Get_Filter_Companies()
        {
            // arrange
            var input = GetCompanyCreateDto();
            input.Name = "Rock";
            await _companyAppService.CreateAsync(input);
            input = GetCompanyCreateDto();
            input.Name = "Ben";
            await _companyAppService.CreateAsync(input);
            var dto = new CompanySearchDto()
            {
                Filter = "Rock"
            };
            //act
            var result = await _companyAppService.GetListAsync(dto);

            // assert
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
        }
        [Fact]
        public async Task Should_Get_Filter_Companies_With_Id()
        {
            // arrange
            var input = GetCompanyCreateDto();
            input.Name = "Rock";
            var newDto = await _companyAppService.CreateAsync(input);
            input = GetCompanyCreateDto();
            input.Name = "Ben";
            await _companyAppService.CreateAsync(input);
            var dto = new CompanySearchDto()
            {
                Ids  = [newDto.Id]
            };
            //act
            var result = await _companyAppService.GetListAsync(dto);

            // assert
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items.First().Id.ShouldBe(newDto.Id);
        }
        [Fact]
        public async Task Should_Clear_Address_And_Contact()
        {
            // arrange
            var input = GetCompanyCreateDto();
            var entry = await _companyAppService.CreateAsync(input);

            var dto = GetCompanyUpdateDto();
            dto.Contacts.Clear();
            dto.Addresses.Clear();

            // act
            await _companyAppService.UpdateAsync(entry.Id, dto);

            // assert
            var target = await _companyAppService.GetAsync(entry.Id);
            target.Addresses.ShouldBeEmpty();
            target.Contacts.ShouldBeEmpty();
        }
        [Fact]
        public async Task Should_Clear_Null_Address_And_Contact()
        {
            // arrange
            var input = GetCompanyCreateDto();
            var entry = await _companyAppService.CreateAsync(input);

            var dto = GetCompanyUpdateDto();
            dto.Contacts = null;
            dto.Addresses = null;

            // act
            await _companyAppService.UpdateAsync(entry.Id, dto);

            // assert
            var target = await _companyAppService.GetAsync(entry.Id);
            target.Addresses.ShouldBeEmpty();
            target.Contacts.ShouldBeEmpty();
        }
    }
}
