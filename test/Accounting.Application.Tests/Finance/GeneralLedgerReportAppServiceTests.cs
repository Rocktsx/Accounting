using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.GeneralLedgerReports;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class GeneralLedgerReportAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IGeneralLedgerReportAppService _glAppService;
        private readonly AccountingTestData _testData;

        public GeneralLedgerReportAppServiceTests()
        {
            _glAppService = GetRequiredService<IGeneralLedgerReportAppService>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Single_Currency_List()
        {
            // arrange    
            var input = new GeneralLedgerReportRequestDto
            {
                StartDate = new DateOnly(_testData.AccountingPeriodYear, 3, 1),
                EndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31),
                PeriodId = _testData.AccountingPeriodYearId,
                SubjectId = _testData.SubjectApId
            };
            // act
            var result = await _glAppService.GetSingleCurrencyListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.LastYearBfOrder);
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.CurrentYearBfOrder);
            result.Count(item => item.SortOrder == AccountingCommonConsts.CurrentPeriodOrder).ShouldBe(3);
        }
        [Fact]
        public async Task Can_Get_Multiple_Currency_List()
        {
            // arrange    
            var input = new GeneralLedgerReportRequestDto
            {
                StartDate = new DateOnly(_testData.AccountingPeriodYear, 3, 10),
                EndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31),
                PeriodId = _testData.AccountingPeriodYearId,
                SubjectId = _testData.SubjectArId
            };
            // act
            var result = await _glAppService.GetMultipleCurrencyListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.LastYearBfOrder);
            result.Count(item => item.SortOrder == AccountingCommonConsts.CurrentYearBfOrder).ShouldBe(2); ;
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.CurrentPeriodOrder);
        }
    }
}
