using Accounting.Finance.ProfitAndLossReports;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class ProfitAndLossReportAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IProfitAndLossReportAppService _plAppService;
        private readonly AccountingTestData _testData;

        public ProfitAndLossReportAppServiceTests()
        {
            _plAppService = GetRequiredService<IProfitAndLossReportAppService>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Mtd_Ytd_List()
        {
            // arrange
            var input = new ProfitAndLossMtdYtdRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                PeriodId = _testData.AccountingPeriodYearId,
            };

            // act
            var result = await _plAppService.GetMtdYtdListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectRentCode
                && item.GroupCode == _testData.AccountTypeE
                && item.SecondaryGroupCode == _testData.AccountTypeAEX);
        }
        [Fact]
        public async Task Can_Get_Ytd_List()
        {
            // arrange
            var input = new ProfitAndLossYearToDateRequestDto
            {
                EndDate = _testData.AccountingPeriodEndDate,
                PeriodId = _testData.AccountingPeriodYearId,
            };

            // act
            var result = await _plAppService.GetYtdListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectRentCode
                && item.GroupCode == _testData.AccountTypeE
                && item.SecondaryGroupCode == _testData.AccountTypeAEX);
        }
        [Fact]
        public async Task Can_Get_12_Months_List()
        {
            // arrange
            var input = new ProfitAndLossMtdYtdRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                PeriodId = _testData.AccountingPeriodYearId,
            };

            // act
            var result = await _plAppService.GetTwelveMonthsListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectRentCode
                && item.GroupCode == _testData.AccountTypeE
                && item.SecondaryGroupCode == _testData.AccountTypeAEX);
            result.First().JanuaryNativeAmount.ShouldBe(-_testData.DocNo1NativeAmount);
        }
    }
}
