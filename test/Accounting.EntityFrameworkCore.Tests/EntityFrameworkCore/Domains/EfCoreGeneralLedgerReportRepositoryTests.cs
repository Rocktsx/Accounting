using Accounting.Finance.Reports;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Accounting.EntityFrameworkCore.Domains
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreGeneralLedgerReportRepositoryTests : AccountingEntityFrameworkCoreTestBase
    {
        private readonly IGeneralLedgerReportRepository _repository;
        private readonly AccountingTestData _testData;

        public EfCoreGeneralLedgerReportRepositoryTests()
        {
            _repository = GetRequiredService<IGeneralLedgerReportRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Single_Currency_List()
        {
            // arrange
            var startDate = new DateOnly(_testData.AccountingPeriodYear, 3, 1);
            var endDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31);
            var periodStartDate = new DateOnly(_testData.AccountingPeriodYear, 1, 1);
            var periodEndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31);

            // act
            var result = await _repository.GetGLSingleCurrencyListAsync(
                startDate, endDate,
                periodStartDate, periodEndDate);

            // assert
            result.ShouldNotBeNull();
            result.ShouldNotContain(item => item.SortOrder == AccountingCommonConsts.LastYearBfOrder);
            result.Count(item => item.SortOrder == AccountingCommonConsts.CurrentYearBfOrder).ShouldBe(3);
            result.Count(item => item.SortOrder == AccountingCommonConsts.CurrentPeriodOrder).ShouldBe(6);
        }
    }
}
