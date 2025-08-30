using Accounting.Finance.Dtos;
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

namespace Accounting.Finance
{
    public class AccountTypeAppService : CrudAppService<AccountType, AccountTypeDto, Guid, FilteredPagedAndSortedResultRequestDto, AccountTypeCreateDto, AccountTypeUpdateDto>, IAccountTypeAppService
    {
       
        public AccountTypeAppService(IRepository<AccountType, Guid> repository) : base(repository)
        {
            GetPolicyName = AccountingPermissions.AccountType;
            DeletePolicyName = AccountingPermissions.AccountTypeDeletion;
        }
        [Authorize(AccountingPermissions.AccountTypeCreation)]
        public override async Task<AccountTypeDto> CreateAsync(AccountTypeCreateDto input)
        {
            var item = new AccountType(GuidGenerator.Create(), input.Code, input.Name, input.OtherName, input.ParentId,
                input.TrialBalanceSort, input.ProfitAndLossSort, input.BalanceSheetSort, input.TrialBalanceGroup,
                input.ProfitAndLossGroup, input.BalanceSheetGroup);
            var entity = await Repository.InsertAsync(item);
            return ObjectMapper.Map<AccountType, AccountTypeDto>(entity);
        }
        [Authorize(AccountingPermissions.AccountType)]
        public override async Task<PagedResultDto<AccountTypeDto>> GetListAsync(FilteredPagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.GetQueryableAsync();
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter) || x.OtherName.Contains(input.Filter));
            var pageQueryable = queryable.Skip(input.SkipCount)
                                .Take(input.MaxResultCount)
                                .OrderBy(input.Sorting ?? nameof(AccountType.Code));
            var list = await AsyncExecuter.ToListAsync(pageQueryable);
            var count = await AsyncExecuter.CountAsync(queryable);

            return new PagedResultDto<AccountTypeDto>(count, ObjectMapper.Map<List<AccountType>, List<AccountTypeDto>>(list));
        }

        public async Task<IEnumerable<AccountTypeSelectDto>> GetSelectListAsync()
        {
            var queryable = await Repository.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable
                .OrderBy(x => x.Code)
                .Select(x => new AccountTypeSelectDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    OtherName = x.OtherName
                }));
        }
        [Authorize(AccountingPermissions.AccountTypeEdit)]
        public override async Task<AccountTypeDto> UpdateAsync(Guid id, AccountTypeUpdateDto input)
        {
            var entity = await Repository.GetAsync(id);
            entity.SetCode(input.Code)
                 .SetName(input.Name)
                 .SetOtherName(input.OtherName)
                 .SetParentId(input.ParentId)
                 .SetBalanceSheetGroup(input.BalanceSheetGroup)
                 .SetBalanceSheetSort(input.BalanceSheetSort)
                 .SetProfitAndLossGroup(input.ProfitAndLossGroup)
                 .SetProfitAndLossSort(input.ProfitAndLossSort)
                 .SetTrialBalanceGroup(input.TrialBalanceGroup)
                 .SetTrialBalanceSort(input.TrialBalanceSort);
            var updateEntity = await Repository.UpdateAsync(entity);
            return ObjectMapper.Map<AccountType, AccountTypeDto>(updateEntity);
        }
    }
}
