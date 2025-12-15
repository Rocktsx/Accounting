using Accounting.Finance.Reports;
using Accounting.Finance.Subjects;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class ProfitAndLossReportRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IProfitAndLossReportRepository _plRepository;
        private readonly AccountingTestData _testData;

        public ProfitAndLossReportRepositoryTests()
        {
            _plRepository = GetRequiredService<IProfitAndLossReportRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }
        [Fact]
        public async Task Can_Get_Year_To_Date_List()
        {
            // arrange
            var endDate = _testData.AccountingPeriodEndDate;
            var periodStartDate = _testData.AccountingPeriodStartDate;

            // act
            var result = await _plRepository.GetYearToDateListAsync(endDate, periodStartDate);

            // assert

            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public async Task Can_Get_Month_To_Date_Year_To_Date_List()
        {
            // arrange
            var startDate = _testData.AccountingPeriodStartDate;
            var endDate = _testData.AccountingPeriodEndDate;
            var periodStartDate = _testData.AccountingPeriodStartDate;

            // act
            var result = await _plRepository.GetMonthToDateAndYearToDateListAsync(startDate, endDate, periodStartDate);

            // assert 
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Can_Get_12_Months_List()
        {
            // arrange
            var startDate = _testData.AccountingPeriodStartDate;
            var endDate = _testData.AccountingPeriodEndDate;
            var periodStartDate = _testData.AccountingPeriodStartDate;

            // act
            var result = await _plRepository.Get12MonthsListAsync(startDate, endDate, periodStartDate);

            // assert 
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
            result.First().JanuaryNativeAmount.ShouldBe(-_testData.DocNo1NativeAmount);
        }
    }
}
