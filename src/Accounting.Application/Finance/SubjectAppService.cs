using Accounting.Common;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.Settings;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance
{
    public class SubjectAppService : CrudAppService<Subject, SubjectDto, SubjectFilterResultDto, Guid,
        SubjectFilterRequestDto, SubjectCreateDto, SubjectUpdateDto>, ISubjectAppService
    {
        public SubjectAppService(ISubjectRepository repository) : base(repository)
        {
            GetPolicyName = AccountingPermissions.Subjects.Default;
            DeletePolicyName = AccountingPermissions.Subjects.Delete;
            GetListPolicyName = AccountingPermissions.Subjects.Default;
        }
        [Authorize(AccountingPermissions.Subjects.Create)]
        public override async Task<SubjectDto> CreateAsync(SubjectCreateDto input)
        {
            var entity = new Subject(GuidGenerator.Create(), input.Code, input.Name, input.OtherName, input.SubjectCategoryId,
               input.AccountTypeId, input.DebitorCreditor, input.CurrencyCode, input.Description, input.IsSubSujectType, input.IsActive,
               input.IsPayMethod, input.SeqCode, CurrentTenant.Id);
            entity = await Repository.InsertAsync(entity);
            return ObjectMapper.Map<Subject, SubjectDto>(entity);
        }
        [Authorize(AccountingPermissions.Subjects.Update)]
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
            entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

            entity = await Repository.UpdateAsync(entity);

            return ObjectMapper.Map<Subject, SubjectDto>(entity);
        }
        protected override async Task<IQueryable<Subject>> CreateFilteredQueryAsync(SubjectFilterRequestDto input)
        {
            var queryable = await (input.IsIncludeAccountType == true ?
                Repository.WithDetailsAsync(item => item.AccountType)
                : Repository.GetQueryableAsync());

            queryable = queryable.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter)
                || x.OtherName.Contains(input.Filter));

            queryable = queryable.WhereIf(input.SubjectCategoryId != null,
                x => x.SubjectCategoryId == input.SubjectCategoryId);

            if (input.IsIncludeReceivableSubject == true || input.IsIncludePayableSubject == true)
            {
                var accountingSettings = LazyServiceProvider.LazyGetRequiredService<IAccountingSettingAppService>();
                if (input.IsIncludeReceivableSubject == true)
                {
                    var receivableSubjectCode = await accountingSettings.GetAccountReceivableSubjectCodeAsync();
                    input.AddSubjectId(receivableSubjectCode);
                }
                if (input.IsIncludePayableSubject == true)
                {
                    var payableSubjectCode = await accountingSettings.GetAccountPayableSubjectCodeAsync();
                    input.AddSubjectId(payableSubjectCode);
                }
            }

            queryable = queryable.WhereIf(input.SubjectIds != null,
                item => input.SubjectIds.Contains(item.Id));

            queryable = queryable.WhereIf(input.IsPaymentMethod != null,
                item => item.IsPayMethod == input.IsPaymentMethod);

            return queryable;
        }
        protected override SubjectFilterResultDto MapToGetListOutputDto(Subject entity)
        {
            var item = base.MapToGetListOutputDto(entity);
            if (entity.AccountType != null)
            {
                item.AccountType = ObjectMapper.Map<AccountType, AccountTypeSimpleDto>(entity.AccountType);
            }
            return item;
        }

        [Authorize(AccountingPermissions.Subjects.Default)]
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

        [Authorize(AccountingPermissions.Subjects.Default)]
        public async Task<IEnumerable<SubjectVoucherSimpleDto>> GetVoucherSimpleListAsync()
        {
            var queryable = await Repository.WithDetailsAsync(item => item.AccountType);
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
                    DebitorCreditor = x.DebitorCreditor,
                    AccountTypeCode = x.AccountType != null ? x.AccountType.Code : null,
                }));
        }

        [Authorize(AccountingPermissions.Subjects.Delete)]
        public override async Task DeleteAsync(Guid id)
        {
            var voucherRepository = LazyServiceProvider.LazyGetRequiredService<IVoucherRepository>();
            if (await voucherRepository.AnyAsync(item => item.Details.Any(detailItem => detailItem.SubjectId == id)))
            {
                throw new BusinessException(AccountingDomainErrorCodes.Subjects.SubjectIsInUse);
            }
            await base.DeleteAsync(id);
        }
        [Authorize(AccountingPermissions.Subjects.Import)]
        public async Task<int> ImportDataAsync(IEnumerable<SubjectImportDto> inputs)
        {
            var codes = await inputs.CheckImportDataAsync(L, item => item.Code,
                async (codes) => (await Repository.GetListAsync(item => codes.Contains(item.Code))).Select(item => item.Code));

            var accountTypeReposity = LazyServiceProvider.GetRequiredService<IAccountTypeRepository>();
            var inputAccTypes = inputs.Where(item => !string.IsNullOrWhiteSpace(item.AccountTypeCode)).Select(item => item.AccountTypeCode).Distinct();
            var accountTypes = (await accountTypeReposity.GetPagedListAsync(codes: inputAccTypes)).ToDictionary(item => item.Code, item => item);

            var categoryReposity = LazyServiceProvider.GetRequiredService<ISubjectCategoryRepository>();
            var inputCategories = inputs.Where(item => !string.IsNullOrWhiteSpace(item.SubjectCategoryCode)).Select(item => item.SubjectCategoryCode).Distinct();
            var categories = (await categoryReposity.GetListAsync(item => inputCategories.Contains(item.Code))).ToDictionary(item => item.Code, item => item);

            var entities = inputs.Where(item => !string.IsNullOrWhiteSpace(item.Code) && !string.IsNullOrWhiteSpace(item.Name))
                    .Select(item =>
                    {
                        var drcr = item.DebitorCreditor == 1 ? DebitorCreditor.Debitor : DebitorCreditor.Creditor;
                        Guid? accTypeId = !string.IsNullOrWhiteSpace(item.AccountTypeCode) && accountTypes.TryGetValue(item.AccountTypeCode, out AccountType? value) ? value.Id : null;
                        Guid? categoryId = !string.IsNullOrWhiteSpace(item.SubjectCategoryCode) && categories.ContainsKey(item.SubjectCategoryCode) ? categories[item.SubjectCategoryCode].Id : null;
                        var entity = new Subject(GuidGenerator.Create(), item.Code, item.Name, item.OtherName, categoryId, accTypeId, drcr,
                            item.CurrencyCode, item.Description, item.IsSubSubjectType, item.IsActive, item.IsPayMethod, item.SeqCode, CurrentTenant.Id);

                        return entity;
                    }).ToList();

            await Repository.InsertManyAsync(entities, true);

            return entities.Count;
        }
    }
}
