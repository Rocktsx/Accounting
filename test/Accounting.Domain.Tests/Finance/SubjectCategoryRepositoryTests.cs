using Accounting.Finance.SubjectCategories;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class SubjectCategoryRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ISubjectCategoryRepository _subjectCategoryRepository;
        private readonly AccountingTestData _testData;

        public SubjectCategoryRepositoryTests()
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
            var result = await _subjectCategoryRepository.GetPagedListAsync(_testData.SubjectCategoryCode,
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
