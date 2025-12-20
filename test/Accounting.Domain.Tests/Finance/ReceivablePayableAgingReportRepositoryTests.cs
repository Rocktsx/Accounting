using Accounting.Finance.Reports;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class ReceivablePayableAgingReportRepositoryTests<TStartupModule> :
        AccountingDomainTestBase<TStartupModule> where TStartupModule : IAbpModule
    {
        private readonly IReceivablePayableAgingReportRepository _agingReportPepository;
        private readonly AccountingTestData _testData;

        public ReceivablePayableAgingReportRepositoryTests()
        {
            _agingReportPepository = GetRequiredService<IReceivablePayableAgingReportRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Aging_Summary_Single_Currency_List()
        {
            // arrange
            var endDate = _testData.AccountingPeriodEndDate;
            ;
            // act
            var result = await _agingReportPepository.GetAgingSummarySingleCurrencyListAsync(
                null, endDate, _testData.AgingDays, AccountTypeTypes.Receivable);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
            var summary = result.First();
            summary.OverdueAmount1.ShouldBe(0);
            summary.OverdueAmount2.ShouldBe(0);
            summary.OverdueAmount3.ShouldBe(0);
            summary.OverdueAmount4.ShouldBe(0);
            summary.OutstandingAmount.ShouldBe(_testData.DocNo1NativeAmount + _testData.DocNo2NativeAmount - _testData.DocNo2PaidNativeAmount);
        }
        [Theory]
        [InlineData(AccountTypeTypes.Receivable)]
        [InlineData(AccountTypeTypes.Payable)]
        public async Task Can_Get_Aging_Detail_List(AccountTypeTypes types)
        {
            // arrange
            var endDate = _testData.AccountingPeriodEndDate;
          
            // act
            var result = await _agingReportPepository.GetAgingDetailListAsync(
                null, endDate, types);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }
        [Fact]
        public async Task Can_Get_Aging_Summary_Multiple_Currency_List()
        {
            // arrange
            var endDate = _testData.AccountingPeriodEndDate;
            ;
            // act
            var result = await _agingReportPepository.GetAgingSummaryMultipleCurrencyListAsync(
                null, endDate, _testData.AgingDays, AccountTypeTypes.Receivable);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }
    }
}
