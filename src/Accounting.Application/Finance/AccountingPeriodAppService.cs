using Accounting.Dtos;
using Accounting.Finance.AccountingPeriods;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace Accounting.Finance
{
    public class AccountingPeriodAppService : AccountingAppService, IAccountingPeriodAppService
    {
        private readonly IAccountingPeriodRepository _accountingPeriodRepository;
        public AccountingPeriodAppService(IAccountingPeriodRepository accountingPeriodRepository)
        {
            _accountingPeriodRepository = accountingPeriodRepository;
        }
        [Authorize(AccountingPermissions.AccountingPeriods.Create)]
        public async Task<AccountingPeriodDto> CreateAsync(AccountingPeriodCreateDto input)
        {
            var item = new AccountingPeriod(GuidGenerator.Create(), input.Code, input.StartDate, input.EndDate, input.IsCurrentPeriod, CurrentTenant.Id);
            var entity = await _accountingPeriodRepository.InsertAsync(item);

            return ObjectMapper.Map<AccountingPeriod, AccountingPeriodDto>(entity);
        }
        [Authorize(AccountingPermissions.AccountingPeriods.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _accountingPeriodRepository.DeleteAsync(id);
        }
        [Authorize(AccountingPermissions.AccountingPeriods.Default)]
        public async Task<AccountingPeriodDto> GetAsync(Guid id)
        {
            var entity = await _accountingPeriodRepository.GetAsync(id);
            return ObjectMapper.Map<AccountingPeriod, AccountingPeriodDto>(entity);
        }
        [Authorize(AccountingPermissions.AccountingPeriods.Default)]
        public async Task<CurrentAccountingPeriodDto> GetCurrentPeriodAsync()
        {
            var list = await _accountingPeriodRepository.GetCurrentPeriodsAsync();
            var queryable = list.GroupBy(item => item.IsCurrentPeriod).Select(grp => new CurrentAccountingPeriodDto()
            {
                StartDate = grp.Min(x => x.StartDate),
                EndDate = grp.Max(x => x.EndDate)
            });
            return queryable.FirstOrDefault() ?? new CurrentAccountingPeriodDto()
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }
        [Authorize(AccountingPermissions.AccountingPeriods.Default)]
        public async Task<PagedResultDto<AccountingPeriodDto>> GetListAsync(FilteredPagedAndSortedResultRequestDto input)
        {
            var list = await _accountingPeriodRepository.GetPagedListAsync(input.Filter, input.Sorting,
                input.MaxResultCount, input.SkipCount);
            var count = await _accountingPeriodRepository.GetCountAsync(input.Filter);

            return new PagedResultDto<AccountingPeriodDto>(count, ObjectMapper.Map<List<AccountingPeriod>, List<AccountingPeriodDto>>([.. list]));
        }
        [Authorize(AccountingPermissions.AccountingPeriods.Update)]
        public async Task UpdateAsync(Guid id, AccountingPeriodUpdateDto input)
        {
            var entity = await _accountingPeriodRepository.GetAsync(id);
            entity.SetCode(input.Code)
                .SetStartDate(input.StartDate)
                .SetEndDate(input.EndDate)
                .SetIsCurrentPeriod(input.IsCurrentPeriod);
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            await _accountingPeriodRepository.UpdateAsync(entity);
        }
    }
}
