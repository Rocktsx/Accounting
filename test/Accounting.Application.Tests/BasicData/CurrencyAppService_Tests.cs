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

namespace Accounting.BasicData
{
    public class CurrencyAppService_Tests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
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
            var id = new CurrencyKey()
            {
                SourceCurrency = "RMB",
                TargetCurrency = "USD"
            };
            // act
            var entity = await currencyAppService.GetAsync(new CurrencyKey() { SourceCurrency = id.SourceCurrency, TargetCurrency = id.TargetCurrency });

            // assert
            entity.ShouldNotBeNull();
            entity.SourceCurrency.ShouldBe(id.SourceCurrency);
            entity.TargetCurrency.ShouldBe(id.TargetCurrency); 
        }
        [Fact]
        public async Task Can_Create_A_Valid_Currency()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // arrange
                var dto = new CurrencyCreateDato()
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
                await currencyAppService.CreateAsync(dto);

                // assert
                var entity = await currencyAppService.GetAsync(new CurrencyKey() { SourceCurrency = dto.SourceCurrency, TargetCurrency = dto.TargetCurrency });

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
                var dto = new CurrencyKey()
                {
                    SourceCurrency = "RMB",
                    TargetCurrency = "USD"
                };
                // act
                await currencyAppService.DeleteAsync(dto);

                // assert 
                var exception = await Assert.ThrowsAsync<EntityNotFoundException>( async () =>
                {
                    await currencyAppService.GetAsync(new CurrencyKey() { SourceCurrency = dto.SourceCurrency, TargetCurrency = dto.TargetCurrency });
                });

                exception.ShouldNotBeNull();
            });
        }
        [Fact]
        public async Task Can_Get_All_Currencies()
        {
            // arrange
           
            // act
            var list = await currencyAppService.GetAllAsync();

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
