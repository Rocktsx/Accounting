using Accounting.BasicData.Dtos;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; 
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories; 

namespace Accounting.BasicData
{
    public class CurrencyAppService : ApplicationService, ICurrencyAppService
    {
        private IRepository<Currency, Guid> _currencyRepository; 
        public CurrencyAppService(IRepository<Currency, Guid> repository)
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
        public async Task<PagedResultDto<CurrencyDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _currencyRepository.GetQueryableAsync();
            queryable = queryable.Skip(input.SkipCount)
                                .Take(input.MaxResultCount)
                                .OrderBy(input.Sorting ?? nameof(Currency.TargetCurrency));
            var list = await AsyncExecuter.ToListAsync(queryable);
            var count = await _currencyRepository.GetCountAsync();

            return new PagedResultDto<CurrencyDto>(count, ObjectMapper.Map<List<Currency>, List<CurrencyDto>>(list));
        }
        [Authorize(AccountingPermissions.Currencies.Update)]
        public async Task UpdateAsync(Guid id, CurrencyUpdateDto input)
        {
            var entity = await _currencyRepository.GetAsync(id);
            entity.SetAmountAndRate(input.SourceAmount, input.TargetAmount, input.ExchangeRate);
            entity.SetEffectiveDate(input.EffectiveDate).SetIsActive(input.IsActive);
            await _currencyRepository.UpdateAsync(entity);
        }
    }
}

