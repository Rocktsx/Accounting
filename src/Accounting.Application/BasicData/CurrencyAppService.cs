using Accounting.BasicData.Currencies;
using Accounting.Dtos;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace Accounting.BasicData
{
    public class CurrencyAppService : ApplicationService, ICurrencyAppService
    {
        private ICurrencyRepository _currencyRepository; 
        public CurrencyAppService(ICurrencyRepository repository)
        {
            _currencyRepository = repository; 
        }
        [Authorize(AccountingPermissions.Currencies.Create)]
        public async Task<CurrencyDto> CreateAsync(CurrencyCreateDto input)
        {
            var newCurrency = new Currency(GuidGenerator.Create(), input.SourceCurrency, input.TargetCurrency,
                input.SourceAmount, input.TargetAmount, input.ExchangeRate, input.EffectiveDate, input.IsActive, CurrentTenant.Id);
            var entity = await _currencyRepository.InsertAsync(newCurrency);
            return ObjectMapper.Map<Currency, CurrencyDto>(entity);
        }
        [Authorize(AccountingPermissions.Currencies.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _currencyRepository.DeleteAsync(id);
        }
        [Authorize(AccountingPermissions.Currencies.Default)]
        public async Task<IEnumerable<CurrencyDto>> GetActiveListAsync()
        {
            var queryable = await _currencyRepository.GetQueryableAsync();
            var list = await AsyncExecuter.ToListAsync(queryable.Where(item => item.IsActive == true));
            return ObjectMapper.Map<List<Currency>, List<CurrencyDto>>(list);
        }
        [Authorize(AccountingPermissions.Currencies.Default)]
        public async Task<CurrencyDto> GetAsync(Guid id)
        {
            var entity = await _currencyRepository.GetAsync(id);
            return ObjectMapper.Map<Currency, CurrencyDto>(entity);
        }
        [Authorize(AccountingPermissions.Currencies.Default)]
        public async Task<PagedResultDto<CurrencyDto>> GetListAsync(FilteredPagedAndSortedResultRequestDto input)
        {
            var queryable = await _currencyRepository.GetQueryableAsync();
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), item => item.TargetCurrency.Contains(input.Filter));
            var pageQueryable = queryable
                                .OrderBy(input.Sorting ?? nameof(Currency.TargetCurrency))
                                .Skip(input.SkipCount)
                                .Take(input.MaxResultCount);
            var list = await AsyncExecuter.ToListAsync(pageQueryable);
            var count = await AsyncExecuter.CountAsync(queryable);

            return new PagedResultDto<CurrencyDto>(count, ObjectMapper.Map<List<Currency>, List<CurrencyDto>>(list));
        }
        [Authorize(AccountingPermissions.Currencies.Update)]
        public async Task UpdateAsync(Guid id, CurrencyUpdateDto input)
        {
            var entity = await _currencyRepository.GetAsync(id);
            entity.SetAmountAndRate(input.SourceAmount, input.TargetAmount, input.ExchangeRate);
            entity.SetEffectiveDate(input.EffectiveDate).SetIsActive(input.IsActive);
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            await _currencyRepository.UpdateAsync(entity);
        }
    }
}

