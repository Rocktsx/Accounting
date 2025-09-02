using Accounting.Finance.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Accounting.Finance
{
    public abstract class SubjectCategoryAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ISubjectCategoryAppService _subjectCategoryAppService;
        private readonly IAccountTypeAppService _accountTypeAppService;
        public SubjectCategoryAppServiceTests()
        {
            _subjectCategoryAppService = GetRequiredService<ISubjectCategoryAppService>();
            _accountTypeAppService = GetRequiredService<IAccountTypeAppService>();
        }
        private async Task<AccountTypeDto> CreateAccountTypeAsync()
        {
            var newdto = new AccountTypeCreateDto()
            {
                Code ="AAAA",
                Name = "AAAA",
                OtherName = "AAAA",
                TrialBalanceGroup =1,
                TrialBalanceSort= 1,
                BalanceSheetGroup= 1,
                BalanceSheetSort=1,
                ProfitAndLossGroup =1,
                ProfitAndLossSort =1
            };
            var dto = await _accountTypeAppService.CreateAsync(newdto);
            return dto;
        } 
        [Fact]
        public async Task Can_Create_A_SubjectCategory()
        {
            // Arrange
            var dto = new SubjectCategoryCreateDto()
            {
                Code = "1001",
                Name = "Cash",
                OtherName = "Cash",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Cash account"
            };
            // Act
            var newDto = await _subjectCategoryAppService.CreateAsync(dto);
            // Assert
            newDto.ShouldNotBeNull();
            newDto.Id.ShouldNotBe(Guid.Empty);
            newDto.Code.ShouldBe(dto.Code);
            newDto.Name.ShouldBe(dto.Name);
            newDto.OtherName.ShouldBe(dto.OtherName);
            newDto.ParentId.ShouldBe(dto.ParentId);
            newDto.DebitorCreditor.ShouldBe(dto.DebitorCreditor);
            newDto.AccountTypeId.ShouldBe(dto.AccountTypeId);
            newDto.ShowDetail.ShouldBe(dto.ShowDetail);
            newDto.Description.ShouldBe(dto.Description);
            newDto.Level.ShouldBe(1);
        }
        [Fact]
        public async Task Cannot_Create_A_SubjectCategory_With_Empty_Code()
        {
            // Arrange
            var dto = new SubjectCategoryCreateDto()
            {
                Code = string.Empty,
                Name = "Cash",
                OtherName = "Cash",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Cash account"
            };
            // Act
            var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _subjectCategoryAppService.CreateAsync(dto);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.ValidationErrors.ShouldNotBeEmpty();
        }
        [Fact]
        public async Task Cannot_Create_A_SubjectCategory_With_Empty_Name()
        {
            // Arrange
            var dto = new SubjectCategoryCreateDto()
            {
                Code = "1001",
                Name = string.Empty,
                OtherName = "Cash",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Cash account"
            };
            // Act
            var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _subjectCategoryAppService.CreateAsync(dto);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.ValidationErrors.ShouldNotBeEmpty();
        }
        [Fact]
        public async Task Can_Get_A_SubjectCategory()
        {
            // Arrange
            var createDto = new SubjectCategoryCreateDto()
            {
                Code = "1001",
                Name = "Cash",
                OtherName = "Cash",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Cash account"
            };
            var newDto = await _subjectCategoryAppService.CreateAsync(createDto);
            // Act
            var fetchedDto = await _subjectCategoryAppService.GetAsync(newDto.Id);
            // Assert
            fetchedDto.ShouldNotBeNull();
            fetchedDto.Id.ShouldBe(newDto.Id);
            fetchedDto.Code.ShouldBe(newDto.Code);
            fetchedDto.Name.ShouldBe(newDto.Name);
            fetchedDto.OtherName.ShouldBe(newDto.OtherName);
            fetchedDto.ParentId.ShouldBe(newDto.ParentId);
            fetchedDto.DebitorCreditor.ShouldBe(newDto.DebitorCreditor);
            fetchedDto.AccountTypeId.ShouldBe(newDto.AccountTypeId);
            fetchedDto.ShowDetail.ShouldBe(newDto.ShowDetail);
            fetchedDto.Description.ShouldBe(newDto.Description);
        }
        [Fact]
        public async Task Cannot_Get_A_Not_Exists_SubjectCategory()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            // Act
            var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await _subjectCategoryAppService.GetAsync(nonExistentId);
            });
            // Assert
            exception.ShouldNotBeNull();
        }
        [Fact]
        public async Task Can_Update_A_SubjectCategory()
        {
            // Arrange
            var createDto = new SubjectCategoryCreateDto()
            {
                Code = "1002",
                Name = "Accounts Receivable",
                OtherName = "AR",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Accounts Receivable account"
            };
            var newDto = await _subjectCategoryAppService.CreateAsync(createDto);
            var updateDto = new SubjectCategoryUpdateDto()
            {
                Code = "1002-Updated",
                Name = "Accounts Receivable Updated",
                OtherName = "AR Updated",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                AccountTypeId = createDto.AccountTypeId,
                ShowDetail = false,
                Description = "Updated description"
            };
            // Act
            var updatedDto = await _subjectCategoryAppService.UpdateAsync(newDto.Id, updateDto);
            // Assert
            updatedDto.ShouldNotBeNull();
            updatedDto.Id.ShouldBe(newDto.Id);
            updatedDto.Code.ShouldBe(updateDto.Code);
            updatedDto.Name.ShouldBe(updateDto.Name);
            updatedDto.OtherName.ShouldBe(updateDto.OtherName);
            updatedDto.ParentId.ShouldBe(updateDto.ParentId);
            updatedDto.DebitorCreditor.ShouldBe(updateDto.DebitorCreditor);
            updatedDto.AccountTypeId.ShouldBe(updateDto.AccountTypeId);
            updatedDto.ShowDetail.ShouldBe(updateDto.ShowDetail);
            updatedDto.Description.ShouldBe(updateDto.Description);
        }
        [Fact]
        public async Task Cannot_Update_A_Not_Exists_SubjectCategory()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var updateDto = new SubjectCategoryUpdateDto()
            {
                Code = "1003",
                Name = "Inventory",
                OtherName = "Inventory",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Inventory account"
            };
            // Act
            var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await _subjectCategoryAppService.UpdateAsync(nonExistentId, updateDto);
            });
            // Assert
            exception.ShouldNotBeNull();
        }
        private async Task<Tuple<SubjectCategoryCreateDto, SubjectCategoryCreateDto, SubjectCategoryDto>> InitGetListDataAsync()
        {
            var accountType = await CreateAccountTypeAsync();
            var dto1 = new SubjectCategoryCreateDto()
            {
                Code = "2001",
                Name = "Sales Revenue",
                OtherName = "Sales",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Creditor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Sales Revenue account"
            };
            var dto2 = new SubjectCategoryCreateDto()
            {
                Code = "2002",
                Name = "Service Revenue",
                OtherName = "Service",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Creditor,
                AccountTypeId = accountType.Id,
                ShowDetail = true,
                Description = "Service Revenue account"
            };
            var createdDto1 = await _subjectCategoryAppService.CreateAsync(dto1);
            dto2.ParentId = createdDto1.Id;
            var createdDto2 = await _subjectCategoryAppService.CreateAsync(dto2);

            return Tuple.Create(dto1, dto2, createdDto2);
        }
        [Fact]
        public async Task Can_Get_SubjectCategories()
        {
            // Arrange 
            var (dto1, dto2, createdDto2) = await InitGetListDataAsync();
            var input = new FilteredPagedAndSortedResultRequestDto()
            {
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = nameof(SubjectCategory.Code),
                Filter = "Revenue"
            };
            // Act
            var result = await _subjectCategoryAppService.GetListAsync(input);
            // Assert
            createdDto2.Level.ShouldBe(2);
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(2);
            result.Items.ShouldContain(x => x.Code == dto1.Code);
            result.Items.ShouldContain(x => x.Code == dto2.Code);
        }
        [Fact]
        public async Task Can_Delete_A_SubjectCategory()
        {
            // Arrange
            var createDto = new SubjectCategoryCreateDto()
            {
                Code = "3001",
                Name = "Miscellaneous Expense",
                OtherName = "Misc Exp",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Debitor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Miscellaneous Expense account"
            };
            var newDto = await _subjectCategoryAppService.CreateAsync(createDto);
            // Act
            await _subjectCategoryAppService.DeleteAsync(newDto.Id);
            // Assert
            var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
            {
                await _subjectCategoryAppService.GetAsync(newDto.Id);
            });
            exception.ShouldNotBeNull();
        }
        [Fact]
        public async Task Can_Get_Simple_Dto_List()
        {
            // Arrange
            var dto1 = new SubjectCategoryCreateDto()
            {
                Code = "4001",
                Name = "Long-term Debt",
                OtherName = "LTD",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Creditor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Long-term Debt account"
            };
            var dto2 = new SubjectCategoryCreateDto()
            {
                Code = "4002",
                Name = "Short-term Debt",
                OtherName = "STD",
                ParentId = null,
                DebitorCreditor = DebitorCreditor.Creditor,
                AccountTypeId = null,
                ShowDetail = true,
                Description = "Short-term Debt account"
            };
            await _subjectCategoryAppService.CreateAsync(dto1);
            await _subjectCategoryAppService.CreateAsync(dto2);
            // Act
            var result = await _subjectCategoryAppService.GetSimpleListAsync();
            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThanOrEqualTo(2);
            result.Any(x => x.Code == dto1.Code).ShouldBeTrue();
            result.Any(x => x.Code == dto2.Code).ShouldBeTrue();
        }
        [Fact]
        public async Task Can_Get_Filter_Query_List()
        {
            // Arrange 
            var (dto1, dto2, _) = await InitGetListDataAsync();
            var input = new FilteredPagedAndSortedResultRequestDto()
            {
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = nameof(SubjectCategory.Code),
                Filter = "Revenue"
            };
            // Act
            var result = await _subjectCategoryAppService.GetFilteredQueryListAsync(input);
            // Assert 
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(2);
            result.Items.ShouldContain(x => x.Code == dto1.Code);
            result.Items.ShouldContain(x => x.Code == dto2.Code && x.ParentCode == dto1.Code);
        }
    }
}
