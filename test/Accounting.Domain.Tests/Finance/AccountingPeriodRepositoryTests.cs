using Accounting.Finance.AccountingPeriods;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class AccountingPeriodRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IAccountingPeriodRepository _periodPepository;
        private readonly AccountingTestData _testData;

        public AccountingPeriodRepositoryTests()
        {
            _periodPepository = GetRequiredService<IAccountingPeriodRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Current_Periods()
        {
            // act
            var result = await _periodPepository.GetCurrentPeriodsAsync();

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Can__Get_Paged_List()
        {
            // arrange
            var filter = _testData.AccountingPeriodYear.ToString();
            // act
            var result = await _periodPepository.GetPagedListAsync(filter);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Can_Get_Count()
        {
            // arrange
            var filter = _testData.AccountingPeriodYear.ToString();
            // act
            var count = await _periodPepository.GetCountAsync(filter);
            // assert
            count.ShouldBe(1);
        }
    }
}
