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
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace Accounting.Finance
{
    public class SubjectAppService : AccountingAppService, ISubjectAppService
    {
        protected ISubjectRepository Repository { get; set; }
        public SubjectAppService(ISubjectRepository repository)
        {
            Repository = repository;
        }
        [Authorize(AccountingPermissions.Subjects.Create)]
        public async Task<SubjectDto> CreateAsync(SubjectCreateDto input)
        {
            var entity = new Subject(GuidGenerator.Create(), input.Code, input.Name, input.OtherName,
                input.SubjectCategoryId, input.AccountTypeId, input.DebitorCreditor, input.CurrencyCode,
                input.Description, input.IsSubSubjectType, input.IsActive, input.IsPayMethod, input.SeqCode, CurrentTenant.Id);
            entity = await Repository.InsertAsync(entity);
            return ObjectMapper.Map<Subject, SubjectDto>(entity);
        }
        [Authorize(AccountingPermissions.Subjects.Update)]
        public async Task<SubjectDto> UpdateAsync(Guid id, SubjectUpdateDto input)
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
        protected async Task HandleFilter(SubjectFilterRequestDto input)
        {
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
        }
        protected SubjectFilterResultDto MapToGetListOutputDto(Subject entity)
        {
            var item = ObjectMapper.Map<Subject, SubjectFilterResultDto>(entity);
            if (entity.AccountType != null)
            {
                item.AccountType = ObjectMapper.Map<AccountType, AccountTypeSimpleDto>(entity.AccountType);
            }
            return item;
        }

        [Authorize(AccountingPermissions.Subjects.Default)]
        public async Task<IEnumerable<SubjectSimpleDto>> GetSimpleListAsync()
        {
            var list = await Repository.GetPagedListAsync(sorting: nameof(Subject.Code));
            return list
                .Select(x => new SubjectSimpleDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    OtherName = x.OtherName
                });
        }

        [Authorize(AccountingPermissions.Subjects.Default)]
        public async Task<IEnumerable<SubjectVoucherSimpleDto>> GetVoucherSimpleListAsync()
        {
            var list = await Repository.GetPagedListAsync(new SubjectFilterRequest { IsIncludeAccountType = true },
                sorting: nameof(Subject.Code));
            return list
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
                });
        }

        [Authorize(AccountingPermissions.Subjects.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            var voucherRepository = LazyServiceProvider.LazyGetRequiredService<IVoucherRepository>();
            if (await voucherRepository.GetCountAsync(subjectId: id) > 0)
            {
                throw new BusinessException(AccountingDomainErrorCodes.Subjects.SubjectIsInUse);
            }
            await Repository.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.Subjects.Default)]
        public async Task<SubjectDto> GetAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            return ObjectMapper.Map<Subject, SubjectDto>(entity);
        }

        [Authorize(AccountingPermissions.Subjects.Default)]
        public async Task<PagedResultDto<SubjectFilterResultDto>> GetListAsync(SubjectFilterRequestDto input)
        {
            HandleFilter(input);
            var request = new SubjectFilterRequest
            {
                Filter = input.Filter,
                SubjectCategoryId = input.SubjectCategoryId,
                SubjectIds = input.SubjectIds,
                IsIncludeAccountType = input.IsIncludeAccountType,
                IsPaymentMethod = input.IsPaymentMethod
            };

            var list = await Repository.GetPagedListAsync(request, input.Sorting,
                input.MaxResultCount, input.SkipCount);
            var totalCount = await Repository.GetCountAsync(request);

            return new PagedResultDto<SubjectFilterResultDto>(
                totalCount,
                [.. list.Select(MapToGetListOutputDto)]
            );
        }

        [Authorize(AccountingPermissions.Subjects.Import)]
        public async Task<int> ImportDataAsync(IEnumerable<SubjectImportDto> inputs)
        {
            var codes = await inputs.CheckImportDataAsync(L, item => item.Code,
                async (codes) => (await Repository.GetPagedListAsync(new SubjectFilterRequest { Codes = codes })).Select(item => item.Code));

            var accountTypeReposity = LazyServiceProvider.GetRequiredService<IAccountTypeRepository>();
            var inputAccTypes = inputs.Where(item => !string.IsNullOrWhiteSpace(item.AccountTypeCode)).Select(item => item.AccountTypeCode).Distinct();
            var accountTypes = (await accountTypeReposity.GetPagedListAsync(codes: inputAccTypes)).ToDictionary(item => item.Code, item => item);

            var categoryReposity = LazyServiceProvider.GetRequiredService<ISubjectCategoryRepository>();
            var inputCategories = inputs.Where(item => !string.IsNullOrWhiteSpace(item.SubjectCategoryCode)).Select(item => item.SubjectCategoryCode).Distinct();
            var categories = (await categoryReposity.GetPagedListAsync(codes: inputCategories)).ToDictionary(item => item.Code, item => item);

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
