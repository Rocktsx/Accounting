using Accounting.Finance.AccountingPeriods;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance
{
    public class AccountingPeriodAppService : AccountingAppService, IAccountingPeriodAppService
    {
        private readonly IRepository<AccountingPeriod, Guid> _accountingPeriodRepository;
        public AccountingPeriodAppService(IRepository<AccountingPeriod, Guid> accountingPeriodRepository)
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
            var queryable = await _accountingPeriodRepository.GetQueryableAsync();
            var newQueryable = queryable.Where(x => x.IsCurrentPeriod).GroupBy(item => 1).Select(grp => new CurrentAccountingPeriodDto()
            {
                StartDate = grp.Min(x => x.StartDate),
                EndDate = grp.Max(x => x.EndDate)
            });
            return await AsyncExecuter.FirstOrDefaultAsync(newQueryable) ?? new CurrentAccountingPeriodDto();
        }
        [Authorize(AccountingPermissions.AccountingPeriods.Default)]
        public async Task<PagedResultDto<AccountingPeriodDto>> GetListAsync(FilteredPagedAndSortedResultRequestDto input)
        {
            var queryable = await _accountingPeriodRepository.GetQueryableAsync();
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x => x.Code.Contains(input.Filter));
            var pageQueryable = queryable.Skip(input.SkipCount)
                                .Take(input.MaxResultCount)
                                .OrderBy(input.Sorting ?? nameof(AccountingPeriod.StartDate));
            var list = await AsyncExecuter.ToListAsync(pageQueryable);
            var count = await AsyncExecuter.CountAsync(queryable);

            return new PagedResultDto<AccountingPeriodDto>(count, ObjectMapper.Map<List<AccountingPeriod>, List<AccountingPeriodDto>>(list));
        }
        [Authorize(AccountingPermissions.AccountingPeriods.Update)]
        public async Task UpdateAsync(Guid id, AccountingPeriodUpdateDto input)
        {
            var entity = await _accountingPeriodRepository.GetAsync(id);
            entity.SetCode(input.Code)
                .SetStartDate(input.StartDate)
                .SetEndDate(input.EndDate)
                .SetIsCurrentPeriod(input.IsCurrentPeriod);
            await _accountingPeriodRepository.UpdateAsync(entity);
        }
    }
}
