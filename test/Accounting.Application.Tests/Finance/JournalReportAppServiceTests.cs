using Accounting.Finance.JournalReports;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class JournalReportAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IJournalReportAppService _journalReportAppService;
        private readonly AccountingTestData _testData;

        public JournalReportAppServiceTests()
        {
            _journalReportAppService = GetRequiredService<IJournalReportAppService>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Single_Currency_Sort_By_Date_List()
        {
            // Arrange
            var request = new JournalReportRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate
            };
            // Act
            var result = await _journalReportAppService.GetSingleCurrencySortByDateListAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedVouchers * 2); 
        }
        [Fact]
        public async Task Can_Get_Single_Currency_Sort_By_Code_List()
        {
            // Arrange
            var request = new JournalReportRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate
            };
            // Act
            var result = await _journalReportAppService.GetSingleCurrencySortByCodeListAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedVouchers * 2);
        }
        [Fact]
        public async Task Can_Get_Multiple_Currency_Sort_By_Date_List()
        {
            // Arrange
            var request = new JournalReportRequestDto
            {
                StartDate = null,
                EndDate = _testData.AccountingPeriodEndDate
            };
            // Act
            var result = await _journalReportAppService.GetMultipleCurrencySortByDateListAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedVouchers * 2);
        }
        [Fact]
        public async Task Can_Get_Multiple_Currency_Sort_By_Code_List()
        {
            // Arrange
            var request = new JournalReportRequestDto
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = null
            };
            // Act
            var result = await _journalReportAppService.GetMultipleCurrencySortByCodeListAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedVouchers * 2); 
        }
    }
}
