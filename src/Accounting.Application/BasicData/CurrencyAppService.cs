using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Accounting.BasicData
{
    public class CurrencyAppService : ApplicationService, ICurrencyAppService
    {
        private IRepository<Currency> _currencyRepository; 
        public CurrencyAppService(IRepository<Currency> repository)
        {
            _currencyRepository = repository; 
        }
        [Authorize(AccountingPermissions.CurrencyCreation)]
        public async Task<CurrencyDto> CreateAsync(CurrencyCreateDto input)
        {
            var newCurrency = new Currency(GuidGenerator.Create(), input.SourceCurrency, input.TargetCurrency,
                input.SourceAmount, input.TargetAmount, input.ExchangeRate, input.EffectiveDate, input.IsActive);
            var entity = await _currencyRepository.InsertAsync(newCurrency);
            return ObjectMapper.Map<Currency, CurrencyDto>(entity);
        }
        [Authorize(AccountingPermissions.CurrencyDeletion)]
        public async Task DeleteAsync(Guid id)
        {
            await _currencyRepository.DeleteAsync(item => item.Id == id);
        }
        [Authorize(AccountingPermissions.Currency)]
        public async Task<IEnumerable<CurrencyDto>> GetActiveListAsync()
        {
            var queryable = await _currencyRepository.GetQueryableAsync();
            var list = await AsyncExecuter.ToListAsync(queryable.Where(item => item.IsActive == true));
            return ObjectMapper.Map<List<Currency>, List<CurrencyDto>>(list);
        }
        [Authorize(AccountingPermissions.Currency)]
        public async Task<CurrencyDto> GetAsync(Guid id)
        {
            var entity = await _currencyRepository.GetAsync(item => item.Id == id);
            return ObjectMapper.Map<Currency, CurrencyDto>(entity);
        }
        [Authorize(AccountingPermissions.Currency)]
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
        [Authorize(AccountingPermissions.CurrencyEdit)]
        public async Task UpdateAsync(Guid id, CurrencyUpdateDto input)
        {
            var entity = await _currencyRepository.GetAsync(item => item.Id == id);
            entity.SetAmountAndRate(input.SourceAmount, input.TargetAmount, input.ExchangeRate);
            entity.SetEffectiveDate(input.EffectiveDate).SetIsActive(input.IsActive);
            await _currencyRepository.UpdateAsync(entity);
        }
    }
}

