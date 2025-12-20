using Accounting.Finance.ReceivableAgingReports;
using Accounting.Finance.Reports;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class PayableAgingReportAppServiceTests<TStartupModule> : 
        AccountingApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
    {
        private readonly IReceivableAgingReportAppService _agingReportAppService;
        private readonly AccountingTestData _testData;

        public PayableAgingReportAppServiceTests()
        {
            _agingReportAppService = GetRequiredService<IReceivableAgingReportAppService>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Aging_Detail_List()
        {
            // arrange
            var input = new AgingReportRequestDto
            {
                EndDate = _testData.AccountingPeriodEndDate
            };

            // act
            var result = await _agingReportAppService.GetAgingDetailListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }
    }
}
