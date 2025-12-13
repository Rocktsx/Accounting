using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class JournalReportRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IJournalReportRepository _journalReportRepository;
        private readonly AccountingTestData _testData;

        public JournalReportRepositoryTests()
        {
            _journalReportRepository =  GetRequiredService<IJournalReportRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Single_Currency_List()
        {
            // arrange
            var request = new JournalReportRequest
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                Prefix = _testData.VoucherPrefix,
                StartNo = 0,
                EndNo = 1000
            };
            // act
            var result = await _journalReportRepository.GetJLSingleCurrencyListAsync(request, 
                sorting: nameof(Voucher.VoucherDate));

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedJournalVouchers * 2);
        }
        [Fact]
        public async Task Can_Get_Multiple_Currency_List()
        {
            // arrange
            var request = new JournalReportRequest
            {
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                Prefix = _testData.VoucherPrefix,
                StartNo = 0,
                EndNo = 100
            };
            // act
            var result = await _journalReportRepository.GetJLSingleCurrencyListAsync(request,
                sorting: nameof(Voucher.VoucherDate));

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedJournalVouchers * 2);
        }
    }
}
