using Accounting.Common;
using Accounting.Dtos;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.SubjectCategories;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance
{
    /// <summary>
    /// 总账类别
    /// </summary>
    public class SubjectCategoryAppService : CrudAppService<SubjectCategory, SubjectCategoryDto, SubjectCategoryFilteredResultDto, Guid,
        SubjectCategoryFilteredRequestDto, SubjectCategoryCreateDto, SubjectCategoryUpdateDto>, ISubjectCategoryAppService
    {
        public SubjectCategoryAppService(ISubjectCategoryRepository repository) : base(repository)
        {
            GetPolicyName = AccountingPermissions.GeneralAccounts.Default;
            DeletePolicyName = AccountingPermissions.GeneralAccounts.Delete;
            GetListPolicyName = AccountingPermissions.GeneralAccounts.Default;
        }
        [Authorize(AccountingPermissions.GeneralAccounts.Create)]
        public override async Task<SubjectCategoryDto> CreateAsync(SubjectCategoryCreateDto input)
        {
            var entity = new SubjectCategory(GuidGenerator.Create(), input.Code, input.Name, input.OtherName, input.ParentId,
               input.DebitorCreditor, input.AccountTypeId, input.ShowDetail, input.Description, CurrentTenant.Id);
            await SetLevel(entity);
            entity = await Repository.InsertAsync(entity);
            return ObjectMapper.Map<SubjectCategory, SubjectCategoryDto>(entity);
        }
        [Authorize(AccountingPermissions.GeneralAccounts.Update)]
        public override async Task<SubjectCategoryDto> UpdateAsync(Guid id, SubjectCategoryUpdateDto input)
        {
            var entity = await Repository.GetAsync(id);
            entity.SetCode(input.Code)
                .SetName(input.Name)
                .SetOtherName(input.OtherName)
                .SetParentId(input.ParentId)
                .SetDebitorCreditor(input.DebitorCreditor)
                .SetAccountTypeId(input.AccountTypeId)
                .SetShowDetail(input.ShowDetail)
                .SetDescription(input.Description);
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            await SetLevel(entity);

            entity = await Repository.UpdateAsync(entity);

            return ObjectMapper.Map<SubjectCategory, SubjectCategoryDto>(entity);
        }
        protected override async Task<IQueryable<SubjectCategory>> CreateFilteredQueryAsync(SubjectCategoryFilteredRequestDto input)
        {
            var queryable = await (input.IsIncludeAccountType == true ?
                    Repository.WithDetailsAsync(item => item.AccountType) : Repository.GetQueryableAsync());
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter) || x.OtherName.Contains(input.Filter));
            return queryable;
        }
        private async Task<IQueryable<SubjectCategory>> NewFilteredQueryAsync(FilteredPagedAndSortedResultRequestDto input, bool withDetails = false)
        {
            var queryable = await (withDetails ? Repository.WithDetailsAsync(item => item.AccountType) : Repository.GetQueryableAsync());
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter) || x.OtherName.Contains(input.Filter));
            return queryable;
        }

        [Authorize(AccountingPermissions.GeneralAccounts.Default)]
        public async Task<IEnumerable<SubjectCategorySimpleDto>> GetSimpleListAsync()
        {
            var queryable = await Repository.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable
                .OrderBy(x => x.Code)
                .Select(x => new SubjectCategorySimpleDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    OtherName = x.OtherName
                }));
        }
        private async Task SetLevel(SubjectCategory category)
        {
            if (category.ParentId == null)
            {
                category.SetLevel(1);
                return;
            }
            try
            {
                var parent = await Repository.GetAsync(category.ParentId.Value);
                category.SetLevel(parent.Level + 1);
            }
            catch (EntityNotFoundException ex)
            {
                Logger.LogException(ex, LogLevel.Information);
                throw new UserFriendlyException(L.GetString("CannotFindParentCategory", category.ParentId));
            }
        }
        public override async Task<PagedResultDto<SubjectCategoryFilteredResultDto>> GetListAsync(SubjectCategoryFilteredRequestDto input)
        {
            var reuslt = await base.GetListAsync(input);
            if (input.IsIncludeParent == true)
            {
                var categoryIds = reuslt.Items.Where(x => x.ParentId != null).Select(x => x.ParentId.Value).ToList();
                var categories = await Repository.GetListAsync(item => categoryIds.Contains(item.Id));
                var categoriesDic = categories.ToDictionary(x => x.Id, x => x);
                foreach (var item in reuslt.Items)
                {
                    if (item.ParentId != null && categoriesDic.TryGetValue(item.ParentId.Value, out SubjectCategory? category))
                    {
                        item.Parent = new SubjectCategorySimpleDto
                        {
                            Code = category.Code,
                            Name = category.Name,
                            OtherName = category.OtherName,
                        };
                    }
                }
            }
            return reuslt;
        }
         
        [Authorize(AccountingPermissions.GeneralAccounts.Import)]
        public async Task<int> ImportDataAsync(IEnumerable<SubjectCategoryImportDto> inputs)
        {
            var codes = await inputs.CheckImportDataAsync(L, item => item.Code,
                async (codes) => (await Repository.GetListAsync(item => codes.Contains(item.Code))).Select(item => item.Code));

            var accountTypeReposity = LazyServiceProvider.GetRequiredService<IAccountTypeRepository>();
            var inputAccTypes = inputs.Select(item => item.AccountTypeCode).Distinct();
            var accountTypes = (await accountTypeReposity.GetListAsync(item => inputAccTypes.Contains(item.Code))).ToDictionary(item => item.Code, item => item);
            var inputCategories = inputs.Select(item => item.ParentCode).Distinct();
            var categories = (await Repository.GetListAsync(item => inputCategories.Contains(item.Code))).ToDictionary(item => item.Code, item => item);
            var inputDics = new Dictionary<string, SubjectCategoryImportDto>(inputs.Count());

            var entities = inputs.Where(item => !string.IsNullOrWhiteSpace(item.Code) && !string.IsNullOrWhiteSpace(item.Name))
                    .Select(item =>
                    {
                        var drcr = item.DebitorCreditor == 1 ? DebitorCreditor.Debitor : DebitorCreditor.Creditor;
                        Guid? accTypeId = !string.IsNullOrWhiteSpace(item.AccountTypeCode) && accountTypes.TryGetValue(item.AccountTypeCode, out AccountType? value) ? value.Id : null;
                        var entity = new SubjectCategory(GuidGenerator.Create(), item.Code, item.Name, item.OtherName, null,
                            drcr, accTypeId, item.ShowDetail, item.Description, CurrentTenant.Id, item.Level);

                        categories[entity.Code] = entity;
                        inputDics.Add(item.Code, item);

                        return entity;
                    }).ToList();
            foreach (var item in entities)
            {
                var inputItem = inputDics[item.Code];
                if (!string.IsNullOrWhiteSpace(inputItem.ParentCode) && categories.TryGetValue(inputItem.ParentCode, out SubjectCategory? parent))
                {
                    item.SetParentId(parent.Id);
                    if (parent.Level + 1 != inputItem.Level)
                    {
                        item.SetLevel(parent.Level + 1);
                    }
                }
            }

            await Repository.InsertManyAsync(entities, true);

            return entities.Count;
        }
    }
}
