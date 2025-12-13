using Accounting.Finance.Reports;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class TrialBalanceReportRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ITrialBalanceReportRepository _tbReportRepository;
        private readonly AccountingTestData _testData;

        public TrialBalanceReportRepositoryTests()
        {
            _tbReportRepository = GetRequiredService<ITrialBalanceReportRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Year_To_Date_List()
        {
            // arrange
            var endDate = _testData.AccountingPeriodEndDate;
            var periodStartDate= _testData.AccountingPeriodStartDate;

            // act
            var result = await _tbReportRepository.GetYearToDateListAsync(endDate, periodStartDate);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.SystemGenGroupSort);
        }

        [Fact]
        public async Task Can_Get_Month_To_Date_Year_To_Date_List()
        {
            // arrange
            var startDate = _testData.AccountingPeriodStartDate;
            var endDate = _testData.AccountingPeriodEndDate;
            var periodStartDate = _testData.AccountingPeriodStartDate;

            // act
            var result = await _tbReportRepository.GetMonthToDateAndYearToDateListAsync(startDate, endDate, periodStartDate);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.SystemGenGroupSort);
        }
    }
}
