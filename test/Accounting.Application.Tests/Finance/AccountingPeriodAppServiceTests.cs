using Accounting.Finance.AccountingPeriods; 
using Shouldly;
using System; 
using System.Threading.Tasks; 
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class AccountingPeriodAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IAccountingPeriodAppService _accountingPeriodAppService;

        public AccountingPeriodAppServiceTests()
        {
            _accountingPeriodAppService = GetRequiredService<IAccountingPeriodAppService>();
        }

        private static AccountingPeriodCreateDto GetCreateDto(int year,bool isCurrentPeriod= false)
        {
            return new AccountingPeriodCreateDto()
            { 
                Code = year.ToString(),
                StartDate = new DateOnly(year, 1, 1),
                EndDate = new DateOnly(year, 12, 31),
                IsCurrentPeriod = isCurrentPeriod
            };
        }
        [Fact]
        public async Task Can_Create_A_AccountingPeriod()
        {
            // Arrange
            var dto = GetCreateDto(2024);

            // Act
            var newDto = await _accountingPeriodAppService.CreateAsync(dto);

            // Assert
            newDto.ShouldNotBeNull(); 
            newDto.Code.ShouldBe(dto.Code);
            newDto.StartDate.ShouldBe(dto.StartDate);
            newDto.EndDate.ShouldBe(dto.EndDate); 
        }

        [Fact]
        public async Task Can_Get_A_Exist_AccountingPeriod()
        {
            // Arrange
            var dto = GetCreateDto(2024);
            var newDto = await _accountingPeriodAppService.CreateAsync(dto);

            // Act
            var existDto = await _accountingPeriodAppService.GetAsync(newDto.Id);

            // Assert
            existDto.ShouldNotBeNull();
            existDto.Id.ShouldBe(newDto.Id);
            existDto.Code.ShouldBe(newDto.Code);
            existDto.StartDate.ShouldBe(newDto.StartDate);
            existDto.EndDate.ShouldBe(newDto.EndDate);
        }
        [Fact]
        public async Task Can_Delete_A_Exist_AccountingPeriod()
        {
            // Arrange
            var dto = GetCreateDto(2024);
            var newDto = await _accountingPeriodAppService.CreateAsync(dto);

            // Act
           await _accountingPeriodAppService.DeleteAsync(newDto.Id);

            // Assert 
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await  _accountingPeriodAppService.GetAsync(newDto.Id);
            });
            exception.ShouldNotBeNull();
        }
        [Fact]
        public async Task Can_Get_AccountingPeriods()
        {
            // Arrange 
            await _accountingPeriodAppService.CreateAsync(GetCreateDto(2022));
            await _accountingPeriodAppService.CreateAsync(GetCreateDto(2023));
            await _accountingPeriodAppService.CreateAsync(GetCreateDto(2024, true));

            var dto = new FilteredPagedAndSortedResultRequestDto() { };

            // Act 
            var result = await _accountingPeriodAppService.GetListAsync(dto);

            // Assert 
            result.Items.Count.ShouldBeGreaterThanOrEqualTo(3);
            result.Items.ShouldContain(item => item.Code == "2024" && item.IsCurrentPeriod == true);
        }
        [Fact]
        public async Task Can_Get_AccountingPeriods_With_Filter()
        {
            // Arrange 
            await _accountingPeriodAppService.CreateAsync(GetCreateDto(2022));
            await _accountingPeriodAppService.CreateAsync(GetCreateDto(2023));
            await _accountingPeriodAppService.CreateAsync(GetCreateDto(2024,true));

            var dto = new FilteredPagedAndSortedResultRequestDto() { Filter ="2024" };

            // Act 
            var result =await _accountingPeriodAppService.GetListAsync(dto);

            // Assert 
            result.Items.Count.ShouldBe(1);
            result.Items.ShouldContain(item => item.Code == "2024" && item.IsCurrentPeriod == true);
        }
        [Fact] 
        public async Task Can_Get_Current_AccountingPeriod()
        {
            // Arrange 
            await _accountingPeriodAppService.CreateAsync(GetCreateDto(2022));
            var dto = await _accountingPeriodAppService.CreateAsync(GetCreateDto(2023, true));
            var secondDto = await _accountingPeriodAppService.CreateAsync(GetCreateDto(2024, true));
            // Act 
            var currentPeriod = await _accountingPeriodAppService.GetCurrentPeriodAsync();
            // Assert  
            currentPeriod.ShouldNotBeNull();
            currentPeriod.StartDate.ShouldBe(dto.StartDate);
            currentPeriod.EndDate.ShouldBe(secondDto.EndDate);
        }
        [Fact]
        public async Task Can_Update_A_Exist_AccountingPeriod()
        {
            // Arrange
            var dto = GetCreateDto(2024);
            var newDto = await _accountingPeriodAppService.CreateAsync(dto);
            var updateDto = new AccountingPeriodUpdateDto()
            {
                Code = "2024-Updated",
                StartDate = new DateOnly(2024, 2, 1),
                EndDate = new DateOnly(2024, 11, 30),
                IsCurrentPeriod = true
            };
            // Act
            await _accountingPeriodAppService.UpdateAsync(newDto.Id, updateDto);
            // Assert
            var existDto = await _accountingPeriodAppService.GetAsync(newDto.Id);
            existDto.ShouldNotBeNull();
            existDto.Id.ShouldBe(newDto.Id);
            existDto.Code.ShouldBe(updateDto.Code);
            existDto.StartDate.ShouldBe(updateDto.StartDate);
            existDto.EndDate.ShouldBe(updateDto.EndDate);
            existDto.IsCurrentPeriod.ShouldBeTrue();
        }
        [Fact]
        public async Task Cannot_Update_A_Not_Exist_AccountingPeriod()
        {
            // Arrange
            var dtoId = Guid.NewGuid();
            var updateDto = new AccountingPeriodUpdateDto()
            {
                Code = "2024-Updated",
                StartDate = new DateOnly(2024, 2, 1),
                EndDate = new DateOnly(2024, 11, 30),
                IsCurrentPeriod = true
            };
            // Act
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            { 
                await _accountingPeriodAppService.UpdateAsync(dtoId, updateDto);
            });
            // Assert
            exception.ShouldNotBeNull(); 
        }
    }
}
