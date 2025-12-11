using Accounting.BasicData.Currencies;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.BasicData
{
    public abstract class CurrencyRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly AccountingTestData _testData;

        public CurrencyRepositoryTests()
        {
            _currencyRepository = GetRequiredService<ICurrencyRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Paged_List()
        {
            // arrange
            var filter = _testData.UsdCurrency;
            // act
            var result = await _currencyRepository.GetPagedListAsync(filter, true);
            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Can_Get_Count()
        {
            // arrange
            var filter = _testData.UsdCurrency;
            // act
            var count = await _currencyRepository.GetCountAsync(filter, true);
            // assert
            count.ShouldBe(1);
        }
    }
}
