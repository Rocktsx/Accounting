using Accounting.Finance.AccountTypes; 
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Accounting.Finance
{
    public abstract class AccountTypeAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IAccountTypeAppService _accountTypeAppService;
        public AccountTypeAppServiceTests()
        {
            _accountTypeAppService = GetRequiredService<IAccountTypeAppService>();
        }
        private static AccountTypeCreateDto GetCreateDto(string code, string name, Guid? parentId = null)
        {
            return new AccountTypeCreateDto()
            {
                Code = code,
                Name = name,
                OtherName = name,
                ParentId = parentId,
                TrialBalanceSort = 0,
                ProfitAndLossSort = 0,
                BalanceSheetSort = 0,
                TrialBalanceGroup = 0,
                ProfitAndLossGroup = 0,
                BalanceSheetGroup = 0
            };
        }
        [Fact]
        public async Task Can_Create_A_AccountType()
        {
            // Arrange
            var dto = GetCreateDto("1000", "Cash");
            // Act
            var newDto = await _accountTypeAppService.CreateAsync(dto);
            // Assert
            newDto.ShouldNotBeNull();
            newDto.Code.ShouldBe(dto.Code);
            newDto.Name.ShouldBe(dto.Name);
            newDto.OtherName.ShouldBe(dto.OtherName);
            newDto.ParentId.ShouldBe(dto.ParentId);
        }
        [Fact]
        public async Task Cannot_Create_AccountType_With_Empty_Code()
        {
            // Arrange
            var dto = GetCreateDto(string.Empty, "Cash");
            // Act
            var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
            { 
                await _accountTypeAppService.CreateAsync(dto);
            });
            // Assert
            exception.ShouldNotBeNull();
        }
        [Fact]
        public async Task Cannot_Create_AccountType_With_Empty_Name()
        {
            // Arrange
            var dto = GetCreateDto("1111222", string.Empty);
            // Act
            var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _accountTypeAppService.CreateAsync(dto);
            });
            // Assert
            exception.ShouldNotBeNull();
        }
        [Fact]
        public async Task Can_Get_A_Exist_AccountType()
        {
            // Arrange
            var dto = GetCreateDto("1000", "Cash");
            var newDto = await _accountTypeAppService.CreateAsync(dto);
            // Act
            var existDto = await _accountTypeAppService.GetAsync(newDto.Id);
            // Assert
            existDto.ShouldNotBeNull();
            existDto.Id.ShouldBe(newDto.Id);
            existDto.Code.ShouldBe(newDto.Code);
            existDto.Name.ShouldBe(newDto.Name);
            existDto.OtherName.ShouldBe(newDto.OtherName);
            existDto.ParentId.ShouldBe(newDto.ParentId);
        }
        [Fact]
        public async Task Can_Get_AccountType_List()
        {
            // Arrange
            var dto1 = GetCreateDto("1000", "Cash11");
            var dto2 = GetCreateDto("2010", "Bank2010");
            await _accountTypeAppService.CreateAsync(dto1);
            await _accountTypeAppService.CreateAsync(dto2);
            await _accountTypeAppService.CreateAsync(GetCreateDto("2022", "Bank2022"));
            await _accountTypeAppService.CreateAsync(GetCreateDto("2033", "Bank2033"));
            await _accountTypeAppService.CreateAsync(GetCreateDto("2044", "Bank2044"));
            // Act
            var list = await _accountTypeAppService.GetListAsync(new FilteredPagedAndSortedResultRequestDto
            {
                MaxResultCount = 2,
                SkipCount = 0,
                Sorting = nameof(AccountTypeDto.Code),
                Filter = "Bank20"
            });
            // Assert
            list.ShouldNotBeNull();
            list.TotalCount.ShouldBe(4);
            list.Items.Count.ShouldBe(2);
            list.Items.ShouldNotContain(x => x.Code == dto1.Code);
            list.Items.ShouldContain(x => x.Code == dto2.Code);
        }
        [Fact]
        public async Task Can_Update_A_AccountType()
        {
            // Arrange
            var dto = GetCreateDto("1000", "Cash");
            var newDto = await _accountTypeAppService.CreateAsync(dto);
            var updateDto = new AccountTypeUpdateDto()
            {
                Code = "1001",
                Name = "Cash Updated",
                OtherName = "Cash Updated",
                ParentId = null,
                TrialBalanceSort = 1,
                ProfitAndLossSort = 1,
                BalanceSheetSort = 1,
                TrialBalanceGroup = 1,
                ProfitAndLossGroup = 1,
                BalanceSheetGroup = 1
            };
            // Act
            var updatedDto = await _accountTypeAppService.UpdateAsync(newDto.Id, updateDto);
            // Assert
            updatedDto.ShouldNotBeNull();
            updatedDto.Id.ShouldBe(newDto.Id);
            updatedDto.Code.ShouldBe(updateDto.Code);
            updatedDto.Name.ShouldBe(updateDto.Name);
            updatedDto.OtherName.ShouldBe(updateDto.OtherName);
            updatedDto.ParentId.ShouldBe(updateDto.ParentId);
        }
        [Fact]
        public async Task Can_Get_AccountType_SelectList()
        {
            // Arrange
            var dto1 = GetCreateDto("1000", "Cash");
            var dto2 = GetCreateDto("2000", "Bank");
            await _accountTypeAppService.CreateAsync(dto1);
            await _accountTypeAppService.CreateAsync(dto2);
            // Act
            var list = await _accountTypeAppService.GetSimpleListAsync();
            // Assert
            list.ShouldNotBeNull();
            list.ShouldContain(x => x.Code == dto1.Code);
            list.ShouldContain(x => x.Code == dto2.Code); 
        }
        [Fact]
        public async Task Can_Delete_A_AccountType()
        {
            // Arrange
            var dto = GetCreateDto("1000", "Cash");
            var newDto = await _accountTypeAppService.CreateAsync(dto);
            // Act
            await _accountTypeAppService.DeleteAsync(newDto.Id);
            // Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _accountTypeAppService.GetAsync(newDto.Id);
            });
            exception.ShouldNotBeNull();
        }
    }
}
