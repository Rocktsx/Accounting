using Accounting.Finance.JournalReports;
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
                StartDate = new DateOnly(_testData.AccountingPeriodYear, 1, 1),
                EndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31)
            };
            // Act
            var result = await _journalReportAppService.GetSingleCurrencySortByDateListAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedVouchers);
            result.Sum(item => item.Details.Count()).ShouldBe(_testData.InsertedVouchers * 2);
        }
        [Fact]
        public async Task Can_Get_Single_Currency_Sort_By_Code_List()
        {
            // Arrange
            var request = new JournalReportRequestDto
            {
                StartDate = new DateOnly(_testData.AccountingPeriodYear, 1, 1),
                EndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31)
            };
            // Act
            var result = await _journalReportAppService.GetSingleCurrencySortByCodeListAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedVouchers);
            result.Sum(item => item.Details.Count()).ShouldBe(_testData.InsertedVouchers * 2);
        }
        [Fact]
        public async Task Can_Get_Multiple_Currency_Sort_By_Date_List()
        {
            // Arrange
            var request = new JournalReportRequestDto
            {
                StartDate = null,
                EndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31)
            };
            // Act
            var result = await _journalReportAppService.GetMultipleCurrencySortByDateListAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedVouchers);
            result.Sum(item => item.Details.Count()).ShouldBe(_testData.InsertedVouchers * 2);
        }
        [Fact]
        public async Task Can_Get_Multiple_Currency_Sort_By_Code_List()
        {
            // Arrange
            var request = new JournalReportRequestDto
            {
                StartDate = new DateOnly(_testData.AccountingPeriodYear, 1, 1),
                EndDate = null
            };
            // Act
            var result = await _journalReportAppService.GetMultipleCurrencySortByCodeListAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedVouchers); 
            result.Sum(item => item.Details.Count()).ShouldBe(_testData.InsertedVouchers * 2);
        }
    }
}
