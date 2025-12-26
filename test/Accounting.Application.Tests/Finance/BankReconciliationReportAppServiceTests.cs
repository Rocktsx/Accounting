using Accounting.Finance.BankReconciliations;
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
    public abstract class BankReconciliationReportAppServiceTests<TStartupModule> : 
        AccountingApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
    {
        private readonly IBankReconciliationReportAppService _appService; 
        private readonly AccountingTestData _testData;
        public BankReconciliationReportAppServiceTests()
        {
            _appService = GetRequiredService<IBankReconciliationReportAppService>();
            _testData = GetRequiredService<AccountingTestData>();
        }
        [Fact]
        public async Task Can_Get_Report_List()
        {
            // arrange
            var input = new BankReconciliationReportRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate
            };

            // act
            var result = await _appService.GetListAsync(input);

            // assert
            result.Count().ShouldBe(_testData.InsertedVouchers);
        }
        [Fact]
        public async Task Can_Get_Unpresented_Report_List()
        {
            // arrange
            var input = new BankReconciliationReportRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate
            };

            // act
            var result = await _appService.GetUnpresentedListAsync(input);

            // assert
            result.Count().ShouldBe(_testData.InsertedVouchers - 1);
        }
    }
}
