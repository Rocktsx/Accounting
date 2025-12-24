using Accounting.Finance.BankReconciliations;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class BankReconciliationRepositoryTests<TStartupModule> : 
        AccountingDomainTestBase<TStartupModule> where TStartupModule : IAbpModule
    {
        private readonly IBankReconciliationRepository _bankReconciliationRepository;
        private readonly AccountingTestData _testData;

        public BankReconciliationRepositoryTests()
        {
            _bankReconciliationRepository = GetRequiredService<IBankReconciliationRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Count()
        {
            // arrange
            var request = new BankReconciliationFilterRequest
            {
                IsPresented = false,
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                StartNo =1,
                EndNo = 100,
                ReferenceNo = _testData.DocNo1,
                SubjectId = _testData.SubjectBankId
            };

            // act
            var result = await _bankReconciliationRepository.GetCountAsync(request);

            // assert
            result.ShouldBe(4);
        }
        [Fact]
        public async Task Can_Get_List()
        {
            // arrange
            var request = new BankReconciliationFilterRequest
            {
                IsPresented = false,
                StartDate = _testData.AccountingPeriodStartDate,
                EndDate = _testData.AccountingPeriodEndDate,
                StartNo = 1,
                EndNo = 100,
                ReferenceNo = string.Empty,
                SubjectId = _testData.SubjectBankId
            };

            // act
            var result = await _bankReconciliationRepository.GetPagedListAsync(request);

            // assert
            result.Count().ShouldBe(4);
        }
    }
}
