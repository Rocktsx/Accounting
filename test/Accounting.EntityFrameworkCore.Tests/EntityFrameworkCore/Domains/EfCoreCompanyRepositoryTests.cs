using Accounting.BasicData.Companies;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Accounting.EntityFrameworkCore.Domains
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreCompanyRepositoryTests : AccountingEntityFrameworkCoreTestBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly AccountingTestData _testData;

        public EfCoreCompanyRepositoryTests()
        {
            _companyRepository = GetRequiredService<ICompanyRepository>();
            _testData = new AccountingTestData();
        }
        [Fact]
        public async Task Can_Get_Company_List()
        {
            // arrange
            var filter = _testData.ClientCode;

            // act
            var result = await _companyRepository.GetPagedListAsync(filter, true);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Can_Get_Count()
        {
            // arrange
            var filter = _testData.ClientCode;

            // act
            var result = await _companyRepository.GetCountAsync(filter, true);

            // assert
            result.ShouldBe(1);
        }

        [Fact]
        public async Task Can_Find_Company()
        {
            // arrange
            var item  = (await _companyRepository.GetPagedListAsync(isClient: true)).FirstOrDefault(); 

            // act
            var result = await _companyRepository.FindAsync(item.Id);

            // assert
            result.ShouldNotBeNull();
            result.Addresses.ShouldNotBeNull();
            result.Addresses.Count().ShouldBe(1);
            result.Contacts.ShouldNotBeNull();
            result.Contacts.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Can_Find_Company_With_No_Detail()
        {
            // arrange
            var item = (await _companyRepository.GetPagedListAsync(isClient: true)).FirstOrDefault();

            // act
            var result = await _companyRepository.FindAsync(item.Id, false);

            // assert
            result.ShouldNotBeNull();
            result.Addresses.ShouldNotBeNull();
            result.Addresses.Count().ShouldBe(0);
            result.Contacts.ShouldNotBeNull();
            result.Contacts.Count().ShouldBe(0);
        }

        [Fact]
        public async Task Can_Get_Last_Number()
        {
            // arrange
            var prefix = _testData.ClientCode;
            // act
            var result = await _companyRepository.GetLastNumberAsync(prefix);
            // assert
            result.ShouldBe(1); 
        }
    }
}
