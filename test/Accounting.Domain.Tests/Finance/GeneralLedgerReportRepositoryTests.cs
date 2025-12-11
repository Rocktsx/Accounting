using Accounting.Finance.Reports;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class GeneralLedgerReportRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IGeneralLedgerReportRepository _repository;
        private readonly AccountingTestData _testData;

        public GeneralLedgerReportRepositoryTests()
        {
            _repository = GetRequiredService<IGeneralLedgerReportRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Single_Currency_List()
        {
            // arrange
            var startDate = new DateOnly(_testData.AccountingPeriodYear, 3, 1);
            var endDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31);
            var periodStartDate = new DateOnly(_testData.AccountingPeriodYear, 1, 1);
            var periodEndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31);

            // act
            var result = await _repository.GetGLSingleCurrencyListAsync(
                startDate, endDate, periodStartDate, periodEndDate);

            // assert
            result.ShouldNotBeNull();
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.LastYearBfOrder);
            result.Count(item => item.SortOrder == AccountingCommonConsts.CurrentYearBfOrder).ShouldBe(3);
            result.Count(item => item.SortOrder == AccountingCommonConsts.CurrentPeriodOrder).ShouldBe(6);
        }
        [Fact]
        public async Task Can_Get_Multiple_Currency_List()
        {
            // arrange
            var startDate = new DateOnly(_testData.AccountingPeriodYear, 3, 1);
            var endDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31);
            var periodStartDate = new DateOnly(_testData.AccountingPeriodYear, 1, 1);
            var periodEndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31);

            // act
            var result = await _repository.GetGLMultipleCurrencyListAsync(
                startDate, endDate, periodStartDate, periodEndDate);

            // assert
            result.ShouldNotBeNull();
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.LastYearBfOrder);
            result.Count(item => item.SortOrder == AccountingCommonConsts.CurrentYearBfOrder).ShouldBe(4);
            result.Count(item => item.SortOrder == AccountingCommonConsts.CurrentPeriodOrder).ShouldBe(6);
        }
    }
}
