using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.SubjectCategories;
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
    public class EfCoreSubjectCategoryRepositoryTests : AccountingEntityFrameworkCoreTestBase
    {
        private readonly ISubjectCategoryRepository _subjectCategoryRepository;
        private readonly AccountingTestData _testData;

        public EfCoreSubjectCategoryRepositoryTests()
        {
            _subjectCategoryRepository = GetRequiredService<ISubjectCategoryRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_List()
        {
            // arrange
            var id = _testData.SubjectCategoryId;

            // act
            var result =await _subjectCategoryRepository.GetPagedListAsync(_testData.SubjectCategoryCode, 
                ids: [id], codes: [_testData.SubjectCategoryCode], isIncludeAccountType: true);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
            result.First().AccountType.ShouldNotBeNull();
        }
        [Fact]
        public async Task Can_Get_Count()
        {
            // arrange
            var id = _testData.SubjectCategoryId;

            // act
            var result = await _subjectCategoryRepository.GetCountAsync(_testData.SubjectCategoryCode,
                ids: [id], codes: [_testData.SubjectCategoryCode]);

            // assert 
            result.ShouldBe(1);
        }
    }
}
