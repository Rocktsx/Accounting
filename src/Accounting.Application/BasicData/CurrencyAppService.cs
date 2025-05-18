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
        public async Task CreateAsync(CurrencyCreateDto input)
        {
            var newCurrency = new Currency(input.SourceCurrency, input.TargetCurrency,
          input.SourceAmount, input.TargetAmount, input.ExchangeRate, input.EffectiveDate, input.IsActive);
            await _currencyRepository.InsertAsync(newCurrency);
        }
        [Authorize(AccountingPermissions.CurrencyDeletion)]
        public async Task DeleteAsync(CurrencyKey id)
        {
            await _currencyRepository.DeleteAsync(item => item.SourceCurrency == id.SourceCurrency && item.TargetCurrency == id.TargetCurrency);
        }
        [Authorize(AccountingPermissions.Currency)]
        public async Task<IEnumerable<CurrencyDto>> GetActiveListAsync()
        {
            var queryable = await _currencyRepository.GetQueryableAsync();
            var list = await AsyncExecuter.ToListAsync(queryable.Where(item => item.IsActive == true)); 
            return ObjectMapper.Map<List<Currency>, List<CurrencyDto>>(list);
        }
        [Authorize(AccountingPermissions.Currency)]
        public async Task<CurrencyDto> GetAsync(CurrencyKey id)
        {
            var entity = await _currencyRepository.GetAsync(item => item.SourceCurrency == id.SourceCurrency && item.TargetCurrency == id.TargetCurrency);
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
        public async Task UpdateAsync(CurrencyKey id, CurrencyUpdateDto input)
        {
            var entity = await _currencyRepository.GetAsync(item => item.SourceCurrency == id.SourceCurrency && item.TargetCurrency == id.TargetCurrency);
            entity.SetAmountAndRate(input.SourceAmount, input.TargetAmount, input.ExchangeRate);
            entity.SetEffectiveDate(input.EffectiveDate).SetIsActive(input.IsActive);
        }
    }
}

