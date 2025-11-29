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
    public class SubjectCategoryAppService : AccountingAppService, ISubjectCategoryAppService
    {
        protected ISubjectCategoryRepository Repository { get; set; }
        public SubjectCategoryAppService(ISubjectCategoryRepository repository)
        {
            Repository = repository;
        } 

        [Authorize(AccountingPermissions.GeneralAccounts.Default)]
        public async Task<SubjectCategoryDto> GetAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            return ObjectMapper.Map<SubjectCategory, SubjectCategoryDto>(entity);
        }

        [Authorize(AccountingPermissions.GeneralAccounts.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await Repository.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.GeneralAccounts.Create)]
        public async Task<SubjectCategoryDto> CreateAsync(SubjectCategoryCreateDto input)
        {
            var entity = new SubjectCategory(GuidGenerator.Create(), input.Code, input.Name, input.OtherName, input.ParentId,
               input.DebitorCreditor, input.AccountTypeId, input.ShowDetail, input.Description, CurrentTenant.Id);
            await SetLevel(entity);
            entity = await Repository.InsertAsync(entity);
            return ObjectMapper.Map<SubjectCategory, SubjectCategoryDto>(entity);
        }

        [Authorize(AccountingPermissions.GeneralAccounts.Update)]
        public async Task<SubjectCategoryDto> UpdateAsync(Guid id, SubjectCategoryUpdateDto input)
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

        [Authorize(AccountingPermissions.GeneralAccounts.Default)]
        public async Task<IEnumerable<SubjectCategorySimpleDto>> GetSimpleListAsync()
        {
            var queryable = await Repository.GetPagedListAsync();
            return queryable
                .OrderBy(x => x.Code)
                .Select(x => new SubjectCategorySimpleDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    OtherName = x.OtherName
                });
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
        protected SubjectCategoryFilteredResultDto MapToGetListOutputDto(SubjectCategory entity)
        {
            var item = ObjectMapper.Map<SubjectCategory, SubjectCategoryFilteredResultDto>(entity);
            if (entity.AccountType != null)
            {
                item.AccountType = ObjectMapper.Map<AccountType, AccountTypeSimpleDto>(entity.AccountType);
            }
            return item;
        }
        public async Task<PagedResultDto<SubjectCategoryFilteredResultDto>> GetListAsync(SubjectCategoryFilteredRequestDto input)
        {
            var list = await Repository.GetPagedListAsync(input.Filter, isIncludeAccountType: input.IsIncludeAccountType?? false,
                sorting: input.Sorting, maxResultCount: input.MaxResultCount, skipCount: input.SkipCount);
            var totalCount = await Repository.GetCountAsync(input.Filter);
            var items = list.Select(MapToGetListOutputDto).ToList();
            var result = new PagedResultDto<SubjectCategoryFilteredResultDto>(totalCount, items);


            if (input.IsIncludeParent == true && result.Items.Count > 0)
            {
                var categoryIds = result.Items.Where(x => x.ParentId != null).Select(x => x.ParentId);
                var categories = await Repository.GetPagedListAsync(ids: categoryIds);
                var categoriesDic = categories.ToDictionary(x => x.Id, x => x);
                foreach (var item in result.Items)
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
            return result;
        }

        [Authorize(AccountingPermissions.GeneralAccounts.Import)]
        public async Task<int> ImportDataAsync(IEnumerable<SubjectCategoryImportDto> inputs)
        {
            var codes = await inputs.CheckImportDataAsync(L, item => item.Code,
                async (codes) => (await Repository.GetPagedListAsync(codes: codes)).Select(item => item.Code));

            var accountTypeReposity = LazyServiceProvider.GetRequiredService<IAccountTypeRepository>();
            var inputAccTypes = inputs.Select(item => item.AccountTypeCode).Distinct();
            var accountTypes = (await accountTypeReposity.GetPagedListAsync(codes: inputAccTypes)).ToDictionary(item => item.Code, item => item);
            var inputCategories = inputs.Select(item => item.ParentCode).Distinct();
            var categories = (await Repository.GetPagedListAsync(codes: inputCategories)).ToDictionary(item => item.Code, item => item);
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
