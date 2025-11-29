using Accounting.Common;
using Accounting.Features;
using Accounting.Finance.Settings;
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
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance;

[RemoteService(false)]
public class VoucherAppService : CrudAppService<Voucher, VoucherDto, Guid,
    VoucherFilterRequestDto, VoucherCreateDto, VoucherUpdateDto>, IVoucherAppService
{
    protected FunctionCodes FunctionCode { get; set; } = FunctionCodes.JournalVoucher;
    public VoucherAppService(IVoucherRepository repository) : base(repository)
    {
    }

    protected async Task ValidateAsync(Voucher voucher, VoucherManager manager)
    {
        await manager.ValidateAsync(voucher);
    }
    protected virtual async Task<string> GetVoucherDateFormatAsync(IAccountingSettingAppService service)
    {
        return await service.GetTransferVoucherDateFormatAsync();
    }
    private async Task<(bool, bool, bool, bool, bool)> GetEnabledFunctionsAsync()
    {
        var enableProjectFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.ProjectFunction);
        var enableRegionFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.RegionFunction);
        var enableDepartmentFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.DepartmentFunction);
        var enableCustom1Function = await FeatureChecker.IsEnabledAsync(AccountingFeatures.Custom1Function);
        var enableCustom2Function = await FeatureChecker.IsEnabledAsync(AccountingFeatures.Custom2Function);

        return (enableProjectFunction, enableRegionFunction, enableDepartmentFunction,
            enableCustom1Function, enableCustom2Function);
    }
    public override async Task<VoucherDto> CreateAsync(VoucherCreateDto input)
    {
        var entity = new Voucher(GuidGenerator.Create(), DateOnly.FromDateTime(input.VoucherDate), input.VoucherType, VoucherStatus.Draft, CurrentTenant.Id);
        var manager = LazyServiceProvider.LazyGetRequiredService<VoucherManager>();
        entity.SetPrefix(input.Prefix);
        entity.SetGenNo(input.GenNo ?? 0);
        var (enableProjectFunction, enableRegionFunction, enableDepartmentFunction,
            enableCustom1Function, enableCustom2Function) = await GetEnabledFunctionsAsync();

        foreach (var item in input.Details)
        {
            var detail = entity.AddDetail(GuidGenerator.Create(), item.SubjectId, item.SubSubjectCode, item.Description,
                item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                item.DocNo, item.DueDate, item.ItemQty ?? 0, item.IsOriginal ?? true, item.PaymentReference);

            VoucherManager.SetFunctionalFields(detail, enableProjectFunction, enableRegionFunction,
                   enableDepartmentFunction, enableCustom1Function, enableCustom2Function,
                   item.Project, item.Region, item.Department, item.Custom1, item.Custom2);
        }

        await ValidateAsync(entity, manager);

        await GenerateCodeAsync(entity);

        entity = await Repository.InsertAsync(entity);
        return ObjectMapper.Map<Voucher, VoucherDto>(entity);
    }
    protected async Task GenerateCodeAsync(Voucher voucher)
    {
        var setting = LazyServiceProvider.LazyGetRequiredService<IAccountingSettingAppService>();
        var voucherDateFormat = await GetVoucherDateFormatAsync(setting);
        var codeGenerator = LazyServiceProvider.LazyGetRequiredService<CodeGenerator>();

        await codeGenerator.GenerateCodeAsync(voucher, new CodeCacheItem
        {
            TenantId = CurrentTenant.Id,
            FunctionCode = FunctionCode
        }, () => VoucherManager.GetPrefix(voucher, voucherDateFormat),
        getLastNumber: async (prefix) =>
        {
            var queryable = await Repository.GetQueryableAsync();
            return await AsyncExecuter.FirstOrDefaultAsync(
                 queryable.Where(item => item.Prefix == prefix).
                 OrderByDescending(item => item.GenNo).Select(
                     item => item.GenNo));
        });
    }

    protected override async Task<Voucher> GetEntityByIdAsync(Guid id)
    {
        var query = (await Repository.WithDetailsAsync(item => item.Details));
        query = query.Where(item => item.Id == id);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query);
        return entity ?? throw new EntityNotFoundException();
    }

    public override async Task<VoucherDto> UpdateAsync(Guid id, VoucherUpdateDto input)
    {
        var entity = await GetEntityByIdAsync(id);
        entity.SetVoucherDate(DateOnly.FromDateTime(input.VoucherDate));
        entity.Details.RemoveAll(item => !input.Details.Any(obj => obj.Id == item.Id));

        var (enableProjectFunction, enableRegionFunction, enableDepartmentFunction,
            enableCustom1Function, enableCustom2Function) = await GetEnabledFunctionsAsync();
        entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        foreach (var item in input.Details)
        {
            if (Guid.Empty.Equals(item.Id))
            {
                var detail = entity.AddDetail(GuidGenerator.Create(), item.SubjectId, item.SubSubjectCode, item.Description,
                    item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                    item.DocNo, item.DueDate, item.ItemQty ?? 0, item.IsOriginal ?? false, item.PaymentReference);

                VoucherManager.SetFunctionalFields(detail, enableProjectFunction, enableRegionFunction,
                    enableDepartmentFunction, enableCustom1Function, enableCustom2Function,
                    item.Project, item.Region, item.Department, item.Custom1, item.Custom2);
            }
            else
            {
                var detail = entity.SetDetail(item.Id, item.SubjectId, item.SubSubjectCode, item.Description,
                    item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                    item.DocNo, item.DueDate, item.ItemQty ?? 0, item.IsOriginal ?? false, item.PaymentReference);

                VoucherManager.SetFunctionalFields(detail, enableProjectFunction, enableRegionFunction,
                    enableDepartmentFunction, enableCustom1Function, enableCustom2Function,
                    item.Project, item.Region, item.Department, item.Custom1, item.Custom2);
            }
        }
        var manager = LazyServiceProvider.LazyGetRequiredService<VoucherManager>();
        await ValidateAsync(entity, manager);
        entity = await Repository.UpdateAsync(entity);
        return ObjectMapper.Map<Voucher, VoucherDto>(entity);
    }

    protected override async Task<IQueryable<Voucher>> CreateFilteredQueryAsync(VoucherFilterRequestDto input)
    {
        var query = await (string.IsNullOrWhiteSpace(input.DocNo)
            ? Repository.GetQueryableAsync()
            : Repository.WithDetailsAsync(item => item.Details));
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), item => item.Code.Contains(input.Filter));
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Prefix), item => item.Prefix == input.Prefix);
        query = query.WhereIf(input.StartNo != null, item => item.GenNo >= input.StartNo);
        query = query.WhereIf(input.EndNo != null, item => item.GenNo <= input.EndNo);
        query = query.WhereIf(input.StartDate != null, item => item.VoucherDate >= input.StartDate);
        query = query.WhereIf(input.EndDate != null, item => item.VoucherDate <= input.EndDate);
        query = query.WhereIf(input.VoucherType != null, item => item.VoucherType == input.VoucherType);
        query = query.WhereIf(input.Status == null, new NoVoidVoucherSpecification());
        query = query.WhereIf(input.Status != null, item => item.Status == input.Status);
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.DocNo), item => item.Details.Any(obj => obj.DocNo.Contains(input.DocNo)));
        return query;
    }

    public virtual async Task UpdateStatus(Guid id, VoucherStatus status)
    {
        var entity = await Repository.GetAsync(id);
        entity.SetStatus(status);

        await Repository.UpdateAsync(entity);
    }

    [Authorize(AccountingPermissions.VoucherStates.UpdateStatus)]
    public virtual async Task UpdateManyStatus(VoucherUpdateStatusDto input, VoucherStatus status)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Code), item => item.Code.Contains(input.Code));
        query = query.WhereIf(input.VoucherType != null, item => item.VoucherType == input.VoucherType);
        query = query.WhereIf(input.Status != null, item => item.Status == input.Status);

        var list = await AsyncExecuter.ToListAsync(query);
        list.ForEach(item => item.SetStatus(status));

        await Repository.UpdateManyAsync(list);
    }

    protected async Task<Dictionary<Guid, Subject>> GetSubjectsAsync(IEnumerable<Guid> ids)
    {
        var subjectRepository = LazyServiceProvider.GetRequiredService<ISubjectRepository>();
        var subjects = await subjectRepository.GetPagedListAsync(new SubjectFilterRequest
        {
            SubjectIds = ids, 
            IsIncludeAccountType = true
        });
        return subjects.ToDictionary(item => item.Id, item => item);
    }
}