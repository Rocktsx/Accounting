using Accounting.Finance.Subjects;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Accounting.Finance
{
    public abstract class SubjectAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ISubjectAppService _subjectAppService;
        public SubjectAppServiceTests()
        {
            _subjectAppService = GetRequiredService<ISubjectAppService>();
        }
        private SubjectCreateDto GetCreateDto(string code, string name, string otherName, string description, int? seqCode)
        {
            return new SubjectCreateDto
            {
                Code = code,
                Name = name,
                OtherName = otherName,
                SubjectCategoryId = null,
                AccountTypeId = null,
                DebitorCreditor = DebitorCreditor.Creditor,
                CurrencyCode = "USD",
                Description = description,
                IsSubSujectType = false,
                IsActive = true,
                IsPayMethod = false,
                SeqCode = seqCode
            };
        }
        private async Task<Tuple<SubjectCreateDto, SubjectCreateDto, SubjectDto, SubjectDto>> InsertNewSubjectsAsync()
        {
            var input1 = GetCreateDto("3001", "Cash", "Cash Account222", "Main cash account", 1);
            var input2 = GetCreateDto("3002", "Bank", "Bank Account222", "Main bank account", 2);
            var dto1 = await _subjectAppService.CreateAsync(input1);
            var dto2 = await _subjectAppService.CreateAsync(input2);

            return Tuple.Create(input1, input2, dto1, dto2);
        }
        [Fact]
        public async Task Can_Create_A_Subject()
        {
            // Arrange
            var input = GetCreateDto("3001", "Cash", "Cash Account222", "Main cash account", 1);
            // Act
            var dto = await _subjectAppService.CreateAsync(input);
            // Assert
            dto.ShouldNotBeNull();
            dto.Code.ShouldBe(input.Code);
            dto.Name.ShouldBe(input.Name);
            dto.OtherName.ShouldBe(input.OtherName);
            dto.SubjectCategoryId.ShouldBe(input.SubjectCategoryId);
            dto.AccountTypeId.ShouldBe(input.AccountTypeId);
            dto.DebitorCreditor.ShouldBe(input.DebitorCreditor);
            dto.CurrencyCode.ShouldBe(input.CurrencyCode);
            dto.Description.ShouldBe(input.Description);
            dto.IsSubSubjectType.ShouldBe(input.IsSubSujectType);
            dto.IsActive.ShouldBe(input.IsActive);
            dto.IsPayMethod.ShouldBe(input.IsPayMethod);
        }
        [Fact]
        public async Task Cannot_Create_A_SubjectC_With_Empty_Code()
        {
            // Arrange
            var input = GetCreateDto(string.Empty, "Cash", "Cash Account222", "Main cash account", 1);
            // Act
            var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _subjectAppService.CreateAsync(input);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.ValidationErrors.ShouldNotBeEmpty();
        }
        [Fact]
        public async Task Cannot_Create_A_Subject_With_Empty_Name()
        {
            // Arrange
            var input = GetCreateDto("30021", string.Empty, "Cash Account", "Main cash account", 1);
            // Act
            var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _subjectAppService.CreateAsync(input);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.ValidationErrors.ShouldNotBeEmpty();
        }
        [Fact]
        public async Task Can_Get_A_Subject()
        {
            // Arrange
            var input = GetCreateDto("30021", "Cash2", "Cash Account33", "Main cash a11ccount", 1); ;
            var dto = await _subjectAppService.CreateAsync(input);
            // Act
            var fetchedDto = await _subjectAppService.GetAsync(dto.Id);
            // Assert
            fetchedDto.ShouldNotBeNull();
            fetchedDto.Id.ShouldBe(dto.Id);
            fetchedDto.Code.ShouldBe(input.Code);
            fetchedDto.Name.ShouldBe(input.Name);
            fetchedDto.OtherName.ShouldBe(input.OtherName);
        }
        [Fact]
        public async Task Can_Update_A_Subject()
        {
            // Arrange
            var input = GetCreateDto("10033", "Receivables", "Accounts Receivable", "Customer receivables", 2);
            var dto = await _subjectAppService.CreateAsync(input);
            var updateInput = new SubjectUpdateDto
            {
                Code = "10034",
                Name = "Updated Receivables",
                OtherName = "Updated Accounts Receivable",
                SubjectCategoryId = null,
                AccountTypeId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                CurrencyCode = "USD",
                Description = "Updated customer receivables",
                IsSubSubjectType = false,
                IsActive = true,
                IsPayMethod = false,
                SeqCode = 4
            };
            // Act
            var updatedDto = await _subjectAppService.UpdateAsync(dto.Id, updateInput);
            // Assert
            updatedDto.ShouldNotBeNull();
            updatedDto.Id.ShouldBe(dto.Id);
            updatedDto.Code.ShouldBe(updateInput.Code);
            updatedDto.Name.ShouldBe(updateInput.Name);
            updatedDto.OtherName.ShouldBe(updateInput.OtherName);
            updatedDto.Description.ShouldBe(updateInput.Description);
            updatedDto.SeqCode.ShouldBe(updateInput.SeqCode);
        }
        [Fact]
        public async Task Can_Delete_A_Subject()
        {
            // Arrange
            var input = GetCreateDto("10033", "Receivables", "Accounts Receivable", "Customer receivables", 2);
            var dto = await _subjectAppService.CreateAsync(input);

            // Act
            await _subjectAppService.DeleteAsync(dto.Id);
            // Assert
            var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await _subjectAppService.GetAsync(dto.Id);
            });
            exception.ShouldNotBeNull();
        }
        [Fact]
        public async Task Can_Get_Subject_List()
        {
            // Arrange
            var items = await InsertNewSubjectsAsync();
            var input1 = items.Item1;
            var input2 = items.Item2;
            // Act
            var result = await _subjectAppService.GetListAsync(new SubjectFilterRequestDto
            {
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = "Code",
                Filter = "Account222",
                IsPaymentMethod = false
            });
            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(2);
            result.Items.ShouldContain(x => x.Code == input1.Code);
            result.Items.ShouldContain(x => x.Code == input2.Code);
        }
        [Fact]
        public async Task Can_Get_Simple_Subject_List()
        {
            // Arrange
            var items = await InsertNewSubjectsAsync();
            var input1 = items.Item1;
            var input2 = items.Item2;
            // Act
            var result = await _subjectAppService.GetSimpleListAsync();
            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThanOrEqualTo(2);
            result.ShouldContain(x => x.Code == input1.Code);
            result.ShouldContain(x => x.Code == input2.Code);
        }
        [Fact]
        public async Task Can_Get_Voucher_Simple_Subject_List()
        {
            // Arrange
            var items = await InsertNewSubjectsAsync();
            var input1 = items.Item1;
            var input2 = items.Item2;
            // Act
            var result = await _subjectAppService.GetVoucherSimpleListAsync();
            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThanOrEqualTo(2);
            result.ShouldContain(x => x.Code == input1.Code);
            result.ShouldContain(x => x.Code == input2.Code);
        }
        [Fact]
        public async Task Can_Get_Filtered_Query_List()
        {
            // Arrange
            var items = await InsertNewSubjectsAsync();
            var input1 = items.Item1;
            var input2 = items.Item2;
            // Act
            var result = await _subjectAppService.GetListAsync(new SubjectFilterRequestDto
            {
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = "Code",
                Filter = "Account222",
                IsIncludeAccountType = true
            });
            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(2);
            result.Items.ShouldContain(x => x.Code == input1.Code);
            result.Items.ShouldContain(x => x.Code == input2.Code);
        }
        [Fact]
        public async Task Can_Get_Filtered_Query_List_With_Id()
        {
            // Arrange
            var items = await InsertNewSubjectsAsync();
            var input1 = items.Item1;
            var dto1 = items.Item3;
            // Act
            var result = await _subjectAppService.GetListAsync(new SubjectFilterRequestDto
            {
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = "Code",
                SubjectIds = [dto1.Id]
            });
            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.TotalCount.ShouldBe(1);
            result.Items.First().Id.ShouldBe(dto1.Id);
        }

        [Fact]
        public async Task Can_Import_Data()
        {
            // Arrange
            var inputs = new List<SubjectImportDto>
            {
                new() {
                    Code ="s1",
                    Name ="s1 name",
                    SubjectCategoryCode ="1",
                    AccountTypeCode ="BAK"
                },
                 new() {
                    Code ="s2",
                    Name ="s2 name",
                    SubjectCategoryCode ="1",
                    AccountTypeCode ="AR"
                }
            };

            // Act
            var result = await _subjectAppService.ImportDataAsync(inputs);

            // Assert
            result.ShouldBe(inputs.Count);
        }
        [Fact]
        public async Task Cannot_Import_Data_With_Duplicate_Code()
        {
            // Arrange
            var inputs = new List<SubjectImportDto>
            {
                new() {
                    Code ="s1",
                    Name ="s1 name",
                    SubjectCategoryCode ="1",
                    AccountTypeCode ="BAK"
                },
                 new() {
                    Code ="s1",
                    Name ="s1 name",
                    SubjectCategoryCode ="1",
                    AccountTypeCode ="AR"
                }
            };

            // Act
            var result = await Should.ThrowAsync<BusinessException>(async () => await _subjectAppService.ImportDataAsync(inputs));

            // Assert
            result.ShouldNotBeNull();
            result.Code.ShouldBe(AccountingDomainErrorCodes.CodeIsDuplicated);
        }
        [Fact]
        public async Task Cannot_Import_Data_With_In_Use_Code()
        {
            // Arrange
            var inputs = new List<SubjectImportDto>
            {
                new() {
                    Code ="s1",
                    Name ="s1 name",
                    SubjectCategoryCode ="1",
                    AccountTypeCode ="BAK"
                },
                 new() {
                    Code ="2801",
                    Name ="2801 name",
                    SubjectCategoryCode ="1",
                    AccountTypeCode ="AR"
                }
            };

            // Act
            var result = await Should.ThrowAsync<BusinessException>(async () => await _subjectAppService.ImportDataAsync(inputs));

            // Assert
            result.ShouldNotBeNull();
            result.Code.ShouldBe(AccountingDomainErrorCodes.CodeIsInUse);
        }
    }
}
