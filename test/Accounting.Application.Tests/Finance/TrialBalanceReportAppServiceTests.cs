using Accounting.Finance.TrialBalanceReports;
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
    public abstract class TrialBalanceReportAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ITrialBalanceReportAppService _tbAppService;
        private readonly AccountingTestData _testData;

        public TrialBalanceReportAppServiceTests()
        {
            _tbAppService = GetRequiredService<ITrialBalanceReportAppService>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Mtd_Ytd_List()
        {
            // arrange
            var input = new TrialBalanceMtdYtdRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                PeriodId = _testData.AccountingPeriodYearId,
            };

            // act
            var result = await _tbAppService.GetMtdYtdListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectBankCode
                && item.GroupCode == _testData.AccountTypeA
                && item.SecondaryGroupCode == _testData.AccountTypeCA);
        }
        [Fact]
        public async Task Can_Get_Ytd_List()
        {
            // arrange
            var input = new TrialBalanceMtdYtdRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                PeriodId = _testData.AccountingPeriodYearId,
            };

            // act
            var result = await _tbAppService.GetYtdListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectBankCode
                && item.GroupCode == _testData.AccountTypeA
                && item.SecondaryGroupCode == _testData.AccountTypeCA);
        }
    }
}
