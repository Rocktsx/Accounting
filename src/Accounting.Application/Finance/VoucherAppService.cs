using Accounting.Finance.Dtos; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance;

[RemoteService(false)]
public class VoucherAppService : CrudAppService<Voucher, VoucherDto, Guid,
    VoucherFilterRequestDto, VoucherCreateDto, VoucherUpdateDto>, IVoucherAppService
{
    public VoucherAppService(IRepository<Voucher, Guid> repository) : base(repository)
    { 
    } 

    protected async Task ValidateAsync(Voucher voucher)
    {
        var manager = LazyServiceProvider.LazyGetRequiredService<VoucherManager>();
        await manager.ValidateAsync(voucher);
        if (voucher.VoucherType == VoucherType.JournalVoucher)
        {
            var subjectRepository =
                LazyServiceProvider.LazyGetRequiredService<IRepository<Subject, Guid>>();
            await manager.ValidateReceivablePayableSubject(voucher, subjectRepository);
        }
    }
    
    public override async Task<VoucherDto> CreateAsync(VoucherCreateDto input)
    {
        var entity = new Voucher(GuidGenerator.Create(), input.VoucherDate, input.VoucherType, VoucherStatus.Draft);
        entity.SetPrefix(input.Prefix);
        entity.SetGenNo(input.GenNo ?? 0);
        foreach (var item in input.Details)
        {
            entity.AddDetail(GuidGenerator.Create(), item.SubjectId, item.SubSubjectCode, item.Description,
                item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                item.DocNo, item.DueDate, item.Project, item.Department, item.Region, item.Custom1, item.Custom2,
                item.ItemQty ?? 0, item.IsOriginal ?? false, item.PaymentReference);
        }

        await ValidateAsync(entity);
        var generateCodeService = LazyServiceProvider.LazyGetRequiredService<GenerateCodeService>();
        await generateCodeService.GenerateCodeAsync(entity, Repository);
        entity = await Repository.InsertAsync(entity);
        return ObjectMapper.Map<Voucher, VoucherDto>(entity);
    }

    protected override async Task<Voucher> GetEntityByIdAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(item => item.Details);
        query = query.Where(item => item.Id == id);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query);
        return entity ?? throw new EntityNotFoundException();
    }

    public override async Task<VoucherDto> UpdateAsync(Guid id, VoucherUpdateDto input)
    {
        var entity = await GetEntityByIdAsync(id);
        entity.SetVoucherDate(input.VoucherDate);
        if(input.Status != null)
        {
           entity.SetStatus(input.Status.Value);
        } 
        entity.Details.RemoveAll(item => !input.Details.Any(obj => obj.Id == item.Id));
        foreach (var item in input.Details)
        {
            if (Guid.Empty.Equals(item.Id))
            {
                entity.AddDetail(GuidGenerator.Create(), item.SubjectId, item.SubSubjectCode, item.Description,
                    item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                    item.DocNo, item.DueDate, item.Project, item.Department, item.Region, item.Custom1, item.Custom2,
                    item.ItemQty ?? 0, item.IsOriginal ?? false, item.PaymentReference);
            }
            else
            {
                entity.SetDetail(item.Id, item.SubjectId, item.SubSubjectCode, item.Description,
                    item.DebitorCreditor, item.CurrencyCode, item.CurrencyRate, item.ForeignAmount, item.NativeAmount,
                    item.DocNo, item.DueDate, item.Project, item.Department, item.Region, item.Custom1, item.Custom2,
                    item.ItemQty ?? 0, item.IsOriginal ?? false, item.PaymentReference);
            }
        }

        await ValidateAsync(entity);
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
        query = query.Where(item =>
            input.Status != null ? item.Status == input.Status : item.Status != VoucherStatus.Void);
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.DocNo),item => item.Details.Any(obj => obj.DocNo.Contains(input.DocNo)));
        return query;
    }
}