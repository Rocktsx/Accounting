using Accounting.Dtos;
using Accounting.Features;
using Accounting.Finance.AccountTypes;
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
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;

namespace Accounting.Finance
{
    /// <summary>
    /// 科目类别
    /// </summary>
    public class AccountTypeAppService : CrudAppService<AccountType, AccountTypeDto, Guid, FilteredPagedAndSortedResultRequestDto, AccountTypeCreateDto, AccountTypeUpdateDto>, IAccountTypeAppService
    {

        public AccountTypeAppService(IAccountTypeRepository repository) : base(repository)
        {
            GetPolicyName = AccountingPermissions.SubjectCategories.Default;
            DeletePolicyName = AccountingPermissions.SubjectCategories.Delete;
            GetListPolicyName = AccountingPermissions.SubjectCategories.Default;
        }

        [RequiresFeature(AccountingFeatures.AccountTypeFunction)]
        [Authorize(AccountingPermissions.SubjectCategories.Create)]
        public override async Task<AccountTypeDto> CreateAsync(AccountTypeCreateDto input)
        {
            var item = new AccountType(GuidGenerator.Create(), input.Code, input.Name, input.OtherName, input.ParentId,
                input.TrialBalanceSort, input.ProfitAndLossSort, input.BalanceSheetSort, input.TrialBalanceGroup,
                input.ProfitAndLossGroup, input.BalanceSheetGroup, CurrentTenant.Id);
            var entity = await Repository.InsertAsync(item);
            return ObjectMapper.Map<AccountType, AccountTypeDto>(entity);
        }
        protected override async Task<IQueryable<AccountType>> CreateFilteredQueryAsync(FilteredPagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.GetQueryableAsync();
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter) || x.OtherName.Contains(input.Filter));
            return queryable;
        }

        [Authorize]
        public async Task<IEnumerable<AccountTypeSimpleDto>> GetSimpleListAsync()
        {
            var queryable = await Repository.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable
                .OrderBy(x => x.Code)
                .Select(x => new AccountTypeSimpleDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    OtherName = x.OtherName
                }));
        }

        [RequiresFeature(AccountingFeatures.AccountTypeFunction)]
        [Authorize(AccountingPermissions.SubjectCategories.Update)]
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
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            var updateEntity = await Repository.UpdateAsync(entity);

            return ObjectMapper.Map<AccountType, AccountTypeDto>(updateEntity);
        }

        [RequiresFeature(AccountingFeatures.AccountTypeFunction)]
        [Authorize(AccountingPermissions.SubjectCategories.Default)]
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }
    }
}
