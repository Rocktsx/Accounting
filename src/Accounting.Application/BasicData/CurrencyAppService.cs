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

        public async Task CreateAsync(CurrencyCreateDato input)
        {
            var newCurrency = new Currency(input.SourceCurrency, input.TargetCurrency,
          input.SourceAmount, input.TargetAmount, input.ExchangeRate, input.EffectiveDate, input.IsActive);
            await _currencyRepository.InsertAsync(newCurrency);
        }

        public async Task DeleteAsync(CurrencyKey id)
        {
            await _currencyRepository.DeleteAsync(item => item.SourceCurrency == id.SourceCurrency && item.TargetCurrency == id.TargetCurrency);
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllAsync()
        {
            var list = await AsyncExecuter.ToListAsync(await _currencyRepository.GetQueryableAsync());
            return ObjectMapper.Map<List<Currency>, List<CurrencyDto>>(list);
        }

        public async Task<CurrencyDto> GetAsync(CurrencyKey id)
        {
            var entity = await _currencyRepository.GetAsync(item => item.SourceCurrency == id.SourceCurrency && item.TargetCurrency == id.TargetCurrency);
            return ObjectMapper.Map<Currency, CurrencyDto>(entity);
        }

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

        public async Task UpdateAsync(CurrencyKey id, CurrencyUpdateDto input)
        {
            var entity = await _currencyRepository.GetAsync(item => item.SourceCurrency == id.SourceCurrency && item.TargetCurrency == id.TargetCurrency);
            entity.SetAmountAndRate(input.SourceAmount, input.TargetAmount, input.ExchangeRate);
            entity.SetEffectiveDate(input.EffectiveDate).SetIsActive(input.IsActive);
        }
    }
}

