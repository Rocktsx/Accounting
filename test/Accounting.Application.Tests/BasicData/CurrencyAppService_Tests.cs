using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit; 
using Accounting.BasicData.Currencies;

namespace Accounting.BasicData
{
    public abstract class CurrencyAppService_Tests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ICurrencyAppService currencyAppService;

        public CurrencyAppService_Tests() {
            currencyAppService = GetRequiredService<ICurrencyAppService>();
        }

        [Fact]
        public async Task Can_Get_An_Exists_Currency()
        {
            // arrange
            var list = await currencyAppService.GetListAsync(new PagedAndSortedResultRequestDto() { MaxResultCount = 10 });
            var dto = list.Items.First();
            var id = dto.Id;
            // act
            var entity = await currencyAppService.GetAsync(id);

            // assert
            entity.ShouldNotBeNull(); 
        }
        [Fact]
        public async Task Can_Create_A_Valid_Currency()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // arrange
                var dto = new CurrencyCreateDto()
                {
                    SourceCurrency = "RMB",
                    TargetCurrency = "EUR",
                    SourceAmount = 750m,
                    TargetAmount = 100m,
                    ExchangeRate = 7.5m,
                    EffectiveDate = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true
                };
                // act
                var newEntity = await currencyAppService.CreateAsync(dto);

                // assert
                var entity = await currencyAppService.GetAsync(newEntity.Id);

                entity.ShouldNotBeNull();
                entity.SourceCurrency.ShouldBe(dto.SourceCurrency);
                entity.TargetCurrency.ShouldBe(dto.TargetCurrency);
                entity.SourceAmount.ShouldBe(dto.SourceAmount);
                entity.TargetAmount.ShouldBe(dto.TargetAmount);
                entity.ExchangeRate.ShouldBe(dto.ExchangeRate);
            });
        }
        [Fact]
        public async Task Can_Delete_A_Exists_Currency()
        {
            await WithUnitOfWorkAsync( async () =>
            {
                // arrange
                var list = await currencyAppService.GetListAsync(new PagedAndSortedResultRequestDto() { MaxResultCount = 10 });
                var dto = list.Items.First();
                // act
                await currencyAppService.DeleteAsync(dto.Id);

                // assert 
                var exception = await Assert.ThrowsAsync<EntityNotFoundException>( async () =>
                {
                    await currencyAppService.GetAsync(dto.Id);
                });

                exception.ShouldNotBeNull();
            });
        }
        [Fact]
        public async Task Can_Get_Active_Currencies()
        {
            // arrange
            var dto = new CurrencyCreateDto()
            {
                SourceCurrency = "RMB",
                TargetCurrency = "EUR",
                SourceAmount = 750m,
                TargetAmount = 100m,
                ExchangeRate = 7.5m,
                EffectiveDate = DateOnly.FromDateTime(DateTime.Now),
                IsActive = false
            };
            await currencyAppService.CreateAsync(dto);

            // act
            var list = await currencyAppService.GetActiveListAsync();

            // assert
            list.Count().ShouldBe(2); 
            list.Any(item => item.TargetCurrency == "USD").ShouldBeTrue();
        }
        [Fact]
        public async Task Can_Get_Currencies()
        {
            // arrange
            var dto = new PagedAndSortedResultRequestDto() { MaxResultCount = 10 };
            // act
            var list = await currencyAppService.GetListAsync(dto);

            // assert
            list.TotalCount.ShouldBe(2);
            list.Items.Any(item => item.TargetCurrency == "USD").ShouldBeTrue();
        }
    }
}
