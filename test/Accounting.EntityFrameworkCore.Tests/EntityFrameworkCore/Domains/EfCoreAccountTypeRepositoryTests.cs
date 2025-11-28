using Accounting.Finance.AccountTypes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Accounting.EntityFrameworkCore.Domains
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreAccountTypeRepositoryTests: AccountingEntityFrameworkCoreTestBase
    {
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly AccountingTestData _testData;

        public EfCoreAccountTypeRepositoryTests()
        {
            _testData = GetRequiredService<AccountingTestData>();
            _accountTypeRepository = GetRequiredService<IAccountTypeRepository>();
        }

        [Fact]
        public async void Can_Get_Paged_List()
        {
            // arrange
            var filter = _testData.AccountTypeBank;
            // act
            var result = await _accountTypeRepository.GetPagedListAsync(filter);
            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async void Can_Get_Paged_List_With_Codes()
        {
            // arrange
            var codes = new List<string> { _testData.AccountTypeBank };
            // act
            var result = await _accountTypeRepository.GetPagedListAsync(codes: codes);
            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async void Can_Get_Count_With_Codes()
        {
            // arrange
            var codes = new List<string> { _testData.AccountTypeBank };
            // act
            var count = await _accountTypeRepository.GetCountAsync(codes: codes);
            // assert
            count.ShouldBe(1);
        }
        [Fact]
        public async void Can_Get_Count()
        {
            // arrange
            var filter = _testData.AccountTypeBank;
            // act
            var count = await _accountTypeRepository.GetCountAsync(filter);
            // assert
            count.ShouldBe(1);
        }
    }
}
