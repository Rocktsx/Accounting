using Accounting.BasicData.Currencies;
using Accounting.Dtos;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;

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
            var list = await _currencyRepository.GetPagedListAsync(isActive: true);
            return ObjectMapper.Map<IEnumerable<Currency>, IEnumerable<CurrencyDto>>(list);
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

            var list = await _currencyRepository.GetPagedListAsync(input.Filter, sorting: input.Sorting,
                maxResultCount: input.MaxResultCount, skipCount: input.SkipCount);
            var count = await _currencyRepository.GetCountAsync(input.Filter);

            return new PagedResultDto<CurrencyDto>(count, ObjectMapper.Map<IEnumerable<Currency>, List<CurrencyDto>>(list));
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

