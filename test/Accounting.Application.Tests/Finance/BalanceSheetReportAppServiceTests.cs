using Accounting.Finance.BalanceSheetReports;
using Shouldly;
using System; 
using System.Linq; 
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class BalanceSheetReportAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IBalanceSheetReportAppService _bsAppService;

        private readonly AccountingTestData _testData;
        public BalanceSheetReportAppServiceTests()
        {
            _bsAppService = GetRequiredService<IBalanceSheetReportAppService>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Mtd_Ytd_List()
        {
            // arrange
            var input = new BalanceSheetMtdYtdRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                PeriodId = _testData.AccountingPeriodYearId,
            };

            // act
            var result = await _bsAppService.GetMtdYtdListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
            result.ShouldContain(item => item.SortOrder == AccountingCommonConsts.SystemGenGroupSort2);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectBankCode
                && item.GroupCode == _testData.AccountTypeA
                && item.SecondaryGroupCode == _testData.AccountTypeCA);
        }
        [Fact]
        public async Task Can_Get_Ytd_List()
        {
            // arrange
            var input = new BalanceSheetYearToDateRequestDto
            {
                EndDate = _testData.AccountingPeriodEndDate,
                PeriodId = _testData.AccountingPeriodYearId,
            };

            // act
            var result = await _bsAppService.GetYtdListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
            result.ShouldContain(item => item.SortOrder == AccountingCommonConsts.SystemGenGroupSort2);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectBankCode
                && item.GroupCode == _testData.AccountTypeA
                && item.SecondaryGroupCode == _testData.AccountTypeCA);
        }
    }
}
