using Accounting.Common;
using Accounting.Features;
using Accounting.Finance.AccountTypes;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Features;

namespace Accounting.Finance
{
    /// <summary>
    /// 科目类别
    /// </summary>
    public class AccountTypeAppService : AccountingAppService, IAccountTypeAppService
    {
        protected IAccountTypeRepository Repository { get; set; }

        public AccountTypeAppService(IAccountTypeRepository repository) 
        {
            Repository = repository;
        }

        [RequiresFeature(AccountingFeatures.AccountTypeFunction)]
        [Authorize(AccountingPermissions.SubjectCategories.Create)]
        public async Task<AccountTypeDto> CreateAsync(AccountTypeCreateDto input)
        {
            var item = new AccountType(GuidGenerator.Create(), input.Code,
                input.Name, input.OtherName, input.ParentId, input.TrialBalanceSort,
                input.ProfitAndLossSort, input.BalanceSheetSort, input.TrialBalanceGroup,
                input.ProfitAndLossGroup, input.BalanceSheetGroup,
                CurrentTenant.Id, input.Category);
            var entity = await Repository.InsertAsync(item);
            return ObjectMapper.Map<AccountType, AccountTypeDto>(entity);
        }

        [RequiresFeature(AccountingFeatures.AccountTypeFunction)]
        [Authorize(AccountingPermissions.SubjectCategories.Default)]
        public  async Task<PagedResultDto<AccountTypeDto>> GetListAsync(AccountTypePagedAndSortedResultRequestDto input)
        {
            var list = await Repository.GetPagedListAsync(input.Filter, sorting: input.Sorting, maxResultCount: input.MaxResultCount, skipCount: input.SkipCount);
            var count = await Repository.GetCountAsync(input.Filter);
            var result = new PagedResultDto<AccountTypeDto>(count, ObjectMapper.Map<IEnumerable<AccountType>, List<AccountTypeDto>>(list));
            if (input.IsIncludeParent == true)
            {
                var codes = result.Items.Where(item =>
                        !item.ParentId.IsEmptyOrNull()).
                        Select(item => item.ParentId);
                var dtos = (await GetSimpleDtoListAsync(codes)).ToDictionary(
                    item => item.Id, item => item);
                foreach (var item in result.Items)
                {
                    if (item.ParentId != null &&
                        dtos.TryGetValue(item.ParentId.Value, out var parent))
                    {
                        item.Parent = parent;
                    }
                }
            }
            return result;
        }

        [Authorize]
        public async Task<IEnumerable<AccountTypeSimpleDto>> GetSimpleListAsync()
        {
            return await GetSimpleDtoListAsync();
        }
        private async Task<IEnumerable<AccountTypeSimpleDto>> GetSimpleDtoListAsync(IEnumerable<Guid?> codes = null)
        { 
            return (await Repository.GetPagedListAsync(ids: codes))
                .OrderBy(x => x.Code)
                .Select(x => new AccountTypeSimpleDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    OtherName = x.OtherName
                });
        }
        [RequiresFeature(AccountingFeatures.AccountTypeFunction)]
        [Authorize(AccountingPermissions.SubjectCategories.Update)]
        public async Task<AccountTypeDto> UpdateAsync(Guid id, AccountTypeUpdateDto input)
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
                 .SetTrialBalanceSort(input.TrialBalanceSort)
                 .SetCategory(input.Category);
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            var updateEntity = await Repository.UpdateAsync(entity);

            return ObjectMapper.Map<AccountType, AccountTypeDto>(updateEntity);
        }

        [RequiresFeature(AccountingFeatures.AccountTypeFunction)]
        [Authorize(AccountingPermissions.SubjectCategories.Delete)]
        public Task DeleteAsync(Guid id)
        {
            return Repository.DeleteAsync(id);
        }

        [RequiresFeature(AccountingFeatures.AccountTypeFunction)]
        [Authorize(AccountingPermissions.SubjectCategories.Default)]
        public async Task<AccountTypeDto> GetAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            return ObjectMapper.Map<AccountType, AccountTypeDto>(entity);
        }
    }
}
