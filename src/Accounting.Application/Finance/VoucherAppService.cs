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
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace Accounting.Finance;

[RemoteService(false)]
public class VoucherAppService : AccountingAppService, IVoucherAppService
{
    protected IVoucherRepository Repository { get; set; }
    protected FunctionCodes FunctionCode { get; set; } = FunctionCodes.JournalVoucher;
    protected string DeletePolicyName { get; set; }
    protected string GetListPolicyName { get; set; }
    protected string GetPolicyName { get; set; }
    protected string UpdatePolicyName { get; set; }
    protected string CreatePolicyName { get; set; }
    protected string UpdateStatuePolicyName { get; set; }

    public VoucherAppService(IVoucherRepository repository)
    {
        Repository = repository;
    }

    protected async Task ValidateAsync(Voucher voucher, VoucherManager manager)
    {
        await manager.ValidateAsync(voucher);
    }
    protected virtual async Task<string> GetVoucherDateFormatAsync(IAccountingSettingAppService service)
    {
        return await service.GetTransferVoucherDateFormatAsync();
    }
    private async Task GetEnabledFunctionsAsync(Voucher voucher)
    {
        var enableProjectFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.ProjectFunction);
        var enableRegionFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.RegionFunction);
        var enableDepartmentFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.DepartmentFunction);
        var enableCustom1Function = await FeatureChecker.IsEnabledAsync(AccountingFeatures.Custom1Function);
        var enableCustom2Function = await FeatureChecker.IsEnabledAsync(AccountingFeatures.Custom2Function);

        voucher.SetFunctionEnable(enableProjectFunction, enableRegionFunction, enableDepartmentFunction,
           enableCustom1Function, enableCustom2Function);
    }
    public virtual async Task<VoucherDto> CreateAsync(VoucherCreateDto input)
    {
        await CheckPolicyAsync(CreatePolicyName);

        var entity = new Voucher(GuidGenerator.Create(), DateOnly.FromDateTime(input.VoucherDate), input.VoucherType, CurrentTenant.Id);
        var manager = LazyServiceProvider.LazyGetRequiredService<VoucherManager>();
        entity.SetPrefix(input.Prefix);
        entity.SetGenNo(input.GenNo ?? 0);
        await GetEnabledFunctionsAsync(entity);

        foreach (var item in input.Details)
        {
            var id = GuidGenerator.Create();
            entity.AddDetail(id, item.SubjectId, item.SubSubjectCode, item.Description,
                item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                item.DocNo, item.DueDate, item.ItemQty ?? 0, item.IsOriginal ?? true, item.PaymentReference,
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
            return (int)await Repository.GetLastNumberAsync(prefix);
        });
    }

    public virtual async Task<VoucherDto> UpdateAsync(Guid id, VoucherUpdateDto input)
    {
        await CheckPolicyAsync(UpdatePolicyName);

        var entity = await Repository.GetAsync(id);
        entity.SetVoucherDate(DateOnly.FromDateTime(input.VoucherDate));
        entity.Details.RemoveAll(item => !input.Details.Any(obj => obj.Id == item.Id));

        await GetEnabledFunctionsAsync(entity);
        entity.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        foreach (var item in input.Details)
        {
            if (Guid.Empty.Equals(item.Id))
            {
                var detailId = GuidGenerator.Create();
                entity.AddDetail(detailId, item.SubjectId, item.SubSubjectCode, item.Description,
                    item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                    item.DocNo, item.DueDate, item.ItemQty ?? 0, item.IsOriginal ?? false, item.PaymentReference,
                    item.Project, item.Region, item.Department, item.Custom1, item.Custom2);
            }
            else
            {
                entity.SetDetail(item.Id, item.SubjectId, item.SubSubjectCode, item.Description,
                    item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                    item.DocNo, item.DueDate, item.ItemQty ?? 0, item.IsOriginal ?? false, item.PaymentReference,
                    item.Project, item.Region, item.Department, item.Custom1, item.Custom2);
            }
        }
        var manager = LazyServiceProvider.LazyGetRequiredService<VoucherManager>();
        await ValidateAsync(entity, manager);
        entity = await Repository.UpdateAsync(entity);
        return ObjectMapper.Map<Voucher, VoucherDto>(entity);
    }

    public virtual async Task UpdateStatus(Guid id, VoucherStatus status)
    {
        await CheckPolicyAsync(UpdateStatuePolicyName);

        var entity = await Repository.GetAsync(id);
        entity.SetStatus(status);

        await Repository.UpdateAsync(entity);
    }

    [Authorize(AccountingPermissions.VoucherStates.UpdateStatus)]
    public virtual async Task UpdateManyStatus(VoucherUpdateStatusDto input, VoucherStatus status)
    {
        var list = (await Repository.GetPagedListAsync(new VoucherFilterRequest
        {
            Filter = input.Code,
            VoucherType = input.VoucherType,
            Status = input.Status
        })).ToList();
        list.ForEach(item => item.SetStatus(status));

        await Repository.UpdateManyAsync(list);
    }

    public virtual async Task<VoucherDto> GetAsync(Guid id)
    {
        await CheckPolicyAsync(GetPolicyName);
        var entity = await Repository.GetAsync(id);

        return ObjectMapper.Map<Voucher, VoucherDto>(entity);
    }

    public async virtual Task<PagedResultDto<VoucherDto>> GetListAsync(VoucherFilterRequestDto input)
    {
        await CheckPolicyAsync(GetListPolicyName);

        var filter = ObjectMapper.Map<VoucherFilterRequestDto, VoucherFilterRequest>(input);

        var list = await Repository.GetPagedListAsync(filter, sorting: input.Sorting, maxResultCount: input.MaxResultCount, skipCount: input.SkipCount);
        var count = await Repository.GetCountAsync(filter);

        return new PagedResultDto<VoucherDto>(count, ObjectMapper.Map<IEnumerable<Voucher>, List<VoucherDto>>(list));
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await CheckPolicyAsync(DeletePolicyName);
        await Repository.DeleteAsync(id);
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