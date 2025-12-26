using Accounting.Finance.BankReconciliations;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class BankReconciliationReportRepositoryTests<TStartupModule> :
        AccountingDomainTestBase<TStartupModule> where TStartupModule : IAbpModule
    {
        private readonly IBankReconciliationReportRepository _reportRepository;

        private readonly AccountingTestData _testData;

        public BankReconciliationReportRepositoryTests()
        {
            _reportRepository = GetRequiredService<IBankReconciliationReportRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }
        [Fact]
        public async Task Can_Get_Report_List()
        {
            // act
            var result = await _reportRepository.GetReportListAsync(_testData.AccountingPeriodStartDate,
                _testData.AccountingPeriodEndDate, _testData.SubjectBankId);

            // assert
            result.Count().ShouldBe(_testData.InsertedVouchers);
        }
        [Fact]
        public async Task Can_Get_Unpresented_Report_List()
        {
            // act
            var result = await _reportRepository.GetUnpresentedReportListAsync(_testData.AccountingPeriodStartDate,
                _testData.AccountingPeriodEndDate, _testData.SubjectBankId);

            // assert
            result.Count().ShouldBe(_testData.InsertedVouchers - 1);
        }
    }
}
