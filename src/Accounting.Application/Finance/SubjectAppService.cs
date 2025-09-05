using Accounting.Finance.Dtos;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance
{
    public class SubjectAppService : CrudAppService<Subject, SubjectDto, Guid,
        SubjectFilterRequestDto, SubjectCreateDto, SubjectUpdateDto>, ISubjectAppService
    {
        public SubjectAppService(IRepository<Subject, Guid> repository) : base(repository)
        {
            GetPolicyName = AccountingPermissions.Subject;
            DeletePolicyName = AccountingPermissions.SubjectDeletion;
            GetListPolicyName = AccountingPermissions.Subject;
        }
        [Authorize(AccountingPermissions.SubjectCreation)]
        public override async Task<SubjectDto> CreateAsync(SubjectCreateDto input)
        {
            var entity = new Subject(GuidGenerator.Create(), input.Code, input.Name, input.OtherName, input.SubjectCategoryId,
               input.AccountTypeId, input.DebitorCreditor, input.CurrencyCode, input.Description, input.IsSubSujectType, input.IsActive,
               input.IsPayMethod, input.SeqCode);
            entity = await Repository.InsertAsync(entity);
            return ObjectMapper.Map<Subject, SubjectDto>(entity);
        }
        [Authorize(AccountingPermissions.SubjectEdit)]
        public override async Task<SubjectDto> UpdateAsync(Guid id, SubjectUpdateDto input)
        {
            var entity = await Repository.GetAsync(id);
            entity.SetCode(input.Code)
                .SetName(input.Name)
                .SetOtherName(input.OtherName)
                .SetSubjectCategoryId(input.SubjectCategoryId)
                .SetAccountTypeId(input.AccountTypeId)
                .SetDebitorCreditor(input.DebitorCreditor)
                .SetCurrencyCode(input.CurrencyCode)
                .SetDescription(input.Description)
                .SetIsSubSubjectType(input.IsSubSubjectType)
                .SetIsActive(input.IsActive)
                .SetIsPayMethod(input.IsPayMethod)
                .SetSeqCode(input.SeqCode);
            entity = await Repository.UpdateAsync(entity);
            return ObjectMapper.Map<Subject, SubjectDto>(entity);
        }
        protected override async Task<IQueryable<Subject>> CreateFilteredQueryAsync(SubjectFilterRequestDto input)
        {
            return await NewFilteredQueryAsync(input);
        }
        private async Task<IQueryable<Subject>> NewFilteredQueryAsync(SubjectFilterRequestDto input, bool withDetails = false)
        {
            var queryable = await (withDetails ? Repository.WithDetailsAsync(item => item.AccountType) : Repository.GetQueryableAsync());
            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter) || x.OtherName.Contains(input.Filter));
            queryable = queryable.WhereIf(input.SubjectCategoryId != null, x => x.SubjectCategoryId == input.SubjectCategoryId);
            return queryable;
        }
        public async Task<IEnumerable<SubjectSimpleDto>> GetSimpleListAsync()
        {
            var queryable = await Repository.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable
                .OrderBy(x => x.Code)
                .Select(x => new SubjectSimpleDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    OtherName = x.OtherName
                }));
        }
        public async Task<IEnumerable<SubjectVoucherSimpleDto>> GetVoucherSimpleListAsync()
        {
            var queryable = await Repository.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable
                .OrderBy(x => x.Code)
                .Select(x => new SubjectVoucherSimpleDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    OtherName = x.OtherName,
                    IsPayMethod = x.IsPayMethod,
                    IsSubSubjectType = x.IsSubSubjectType,
                    CurrencyCode = x.CurrencyCode,
                    DebitorCreditor = x.DebitorCreditor
                }));
        }
        [Authorize(AccountingPermissions.Subject)]
        public async Task<PagedResultDto<SubjectFilteredQueryDto>> GetFilteredQueryListAsync(SubjectFilterRequestDto input)
        {
            var queryable = await NewFilteredQueryAsync(input, true);
            var totalCount = await AsyncExecuter.CountAsync(queryable);
            queryable = ApplySorting(queryable, input);
            var newQueryable = ApplyPaging(queryable, input).Select(item => new SubjectFilteredQueryDto
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                OtherName = item.OtherName,
                SubjectCategoryId = item.SubjectCategoryId,
                AccountTypeId = item.AccountTypeId, 
                DebitorCreditor = item.DebitorCreditor,
                CurrencyCode = item.CurrencyCode,
                Description = item.Description,
                IsSubSubjectType = item.IsSubSubjectType,
                IsActive = item.IsActive,
                IsPayMethod = item.IsPayMethod,
                SeqCode = item.SeqCode,
                AccountTypeCode = item.AccountType != null ? item.AccountType.Code : null,
                AccountTypeName = item.AccountType != null ? item.AccountType.Name : null,
                AccountTypeOtherName = item.AccountType != null ? item.AccountType.OtherName : null,
            });
            var dtos = await AsyncExecuter.ToListAsync(newQueryable);
            return new PagedResultDto<SubjectFilteredQueryDto>(totalCount, dtos );
        }
    }
}
