using Accounting.Finance.Reports;
using Accounting.Finance.Subjects;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class BalanceSheetReportRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IBalanceSheetReportRepository _bsRepository;
        private readonly AccountingTestData _testData;
        private readonly ISubjectRepository _subjectRepository;

        public BalanceSheetReportRepositoryTests()
        {
            _bsRepository = GetRequiredService<IBalanceSheetReportRepository>();
            _testData = GetRequiredService<AccountingTestData>();
            _subjectRepository = GetRequiredService<ISubjectRepository>();
        }

        [Fact]
        public async Task Can_Get_Year_To_Date_List()
        {
            // arrange
            var endDate = _testData.AccountingPeriodEndDate;
            var periodStartDate = _testData.AccountingPeriodStartDate;

            // act
            var result = await _bsRepository.GetYearToDateListAsync(endDate, periodStartDate);

            // assert
            var bankSubject = await _subjectRepository.GetAsync(_testData.SubjectBankId);

            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
            result.ShouldContain(item => item.SortOrder == AccountingCommonConsts.SystemGenGroupSort2);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectBankCode
                && item.AccountTypeId == bankSubject.AccountTypeId);
        }

        [Fact]
        public async Task Can_Get_Month_To_Date_Year_To_Date_List()
        {
            // arrange
            var startDate = _testData.AccountingPeriodStartDate;
            var endDate = _testData.AccountingPeriodEndDate;
            var periodStartDate = _testData.AccountingPeriodStartDate;

            // act
            var result = await _bsRepository.GetMonthToDateAndYearToDateListAsync(startDate, endDate, periodStartDate);

            // assert
            var bankSubject = await _subjectRepository.GetAsync(_testData.SubjectBankId);
            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
            result.ShouldContain(item => item.SortOrder == AccountingCommonConsts.SystemGenGroupSort2);
            result.ShouldContain(item => item.SubjectCode == _testData.SubjectBankCode
               && item.AccountTypeId == bankSubject.AccountTypeId);
        } 
    }
}
