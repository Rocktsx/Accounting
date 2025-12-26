using Accounting.Finance.Subjects;
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
    public abstract class SubjectRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ISubjectRepository _subjectPepository;
        private readonly AccountingTestData _testData;

        public SubjectRepositoryTests()
        {
            _subjectPepository = GetRequiredService<ISubjectRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Count()
        {
            // act
            var count = await _subjectPepository.GetCountAsync();

            // assert
            count.ShouldBe(_testData.InsertedSubjectCount);
        }
        [Fact]
        public async Task Can_Get_Count_With_Filter()
        {
            // arrange
            var request = new SubjectFilterRequest
            {
                Filter = _testData.SubjectArCode,
                SubjectIds = [_testData.SubjectArId],
                Codes = [_testData.SubjectArCode],
                SubjectCategoryId = _testData.SubjectCategoryId,
                IsIncludeAccountType = true,
                IsPaymentMethod = true
            };
            // act
            var count = await _subjectPepository.GetCountAsync(request);

            // assert
            count.ShouldBe(1);
        }
        [Fact]
        public async Task Can_Get_List()
        {
            // act
            var result = await _subjectPepository.GetPagedListAsync();

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(_testData.InsertedSubjectCount);
        }
        [Fact]
        public async Task Can_Get_List_With_Filter()
        {
            // arrange
            var request = new SubjectFilterRequest
            {
                Filter = _testData.SubjectArCode,
                SubjectIds = [_testData.SubjectArId],
                Codes = [_testData.SubjectArCode],
                SubjectCategoryId = _testData.SubjectCategoryId,
                IsIncludeAccountType = true,
                IsPaymentMethod = true
            };
            // act
            var result = await _subjectPepository.GetPagedListAsync(request);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Can_Get_List_With_Category_Filter()
        {
            // arrange
            var request = new SubjectFilterRequest
            {
                AccountTypeCategory = AccountTypeTypes.Bank
            };
            // act
            var result = await _subjectPepository.GetPagedListAsync(request);

            // assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
    }
}
