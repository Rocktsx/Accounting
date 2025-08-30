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
    public class SubjectCategoryAppService : CrudAppService<SubjectCategory, SubjectCategoryDto, Guid,
        FilteredPagedAndSortedResultRequestDto, SubjectCategoryCreateDto, SubjectCategoryUpdateDto>, ISubjectCategoryAppService
    {
        public SubjectCategoryAppService(IRepository<SubjectCategory, Guid> repository) : base(repository)
        {
            GetPolicyName = AccountingPermissions.SubjectCategory;
            DeletePolicyName = AccountingPermissions.AccountTypeDeletion;
        }
        [Authorize(AccountingPermissions.SubjectCategoryCreation)]
        public override async Task<SubjectCategoryDto> CreateAsync(SubjectCategoryCreateDto input)
        {
             var entity = new SubjectCategory(GuidGenerator.Create(), input.Code, input.Name, input.OtherName, input.ParentId,
                input.CreditDebit, input.AccountTypeId, input.ShowDetail, input.Description);
            entity = await Repository.InsertAsync(entity);
            return ObjectMapper.Map<SubjectCategory, SubjectCategoryDto>(entity);
        }
        [Authorize(AccountingPermissions.SubjectCategoryEdit)]
        public override async Task<SubjectCategoryDto> UpdateAsync(Guid id, SubjectCategoryUpdateDto input)
        {
            var entity = await Repository.GetAsync(id);
            entity.SetCode(input.Code)
                .SetName(input.Name)
                .SetOtherName(input.OtherName)
                .SetParentId(input.ParentId)
                .SetCreditDebit(input.CreditDebit)
                .SetAccountTypeId(input.AccountTypeId)
                .SetShowDetail(input.ShowDetail)
                .SetDescription(input.Description);
            entity = await Repository.UpdateAsync(entity);
            return ObjectMapper.Map<SubjectCategory, SubjectCategoryDto>(entity);
        }
        [Authorize(AccountingPermissions.SubjectCategory)]
        public override async Task<PagedResultDto<SubjectCategoryDto>> GetListAsync(FilteredPagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.GetQueryableAsync();
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter) || x.OtherName.Contains(input.Filter));
            var pageQueryable = queryable.Skip(input.SkipCount)
                                .Take(input.MaxResultCount)
                                .OrderBy(input.Sorting ?? nameof(SubjectCategory.Code));
            var list = await AsyncExecuter.ToListAsync(pageQueryable);
            var count = await AsyncExecuter.CountAsync(queryable);
            return new PagedResultDto<SubjectCategoryDto>(count, ObjectMapper.Map<List<SubjectCategory>, List<SubjectCategoryDto>>(list));
        }
    }
}
