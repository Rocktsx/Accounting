using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance;

public class Voucher : AuditedAggregateRootWithCode<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public DateOnly VoucherDate { get; private set; }
    public VoucherType VoucherType { get; private set; }
    public VoucherStatus Status { get; private set; }

    public virtual ICollection<VoucherDetail> Details { get; private set; } = [];

    private Voucher()
    {
    }

    public Voucher(Guid id, DateOnly voucherDate, VoucherType voucherType, VoucherStatus status)
    {
        Id = id;
        SetVoucherDate(voucherDate);
        SetVoucherType(voucherType);
        SetStatus(status);
    }

    public Voucher SetVoucherDate(DateOnly voucherDate)
    { 
        VoucherDate = voucherDate;
        return this;
    }

    public Voucher SetVoucherType(VoucherType voucherType)
    {
        VoucherType = voucherType;
        return this;
    }

    public Voucher SetStatus(VoucherStatus status)
    {
        Status = status;
        return this;
    }

    public Voucher AddDetail(Guid id, Guid subjectId, string subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docNo, DateOnly? dueDate, string project, string department,
        string region, string custom1, string custom2, decimal itemQty, bool isOriginal)
    {
        var item = new VoucherDetail(id, this.Id, subjectId, subSubjectCode, description, debitorCreditor, currencyCode,
            currencyRate, foreignAmount, nativeAmount, docNo, dueDate, project, department, region, custom1,
            custom2, itemQty, isOriginal);
        Details.Add(item);
        return this;
    }

    public Voucher SetDetail(Guid id, Guid subjectId, string subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docNo, DateOnly? dueDate, string project, string department,
        string region, string custom1, string custom2, decimal itemQty, bool isOriginal)
    {
        var item = Details.FirstOrDefault(obj => obj.Id == id);
        if (item == null)
        {
            return this;
        }

        item.SetSubjectId(subjectId)
            .SetDebitorCreditor(debitorCreditor)
            .SetSubSubjectCode(subSubjectCode)
            .SetDescription(description)
            .SetCurrencyAndAmount(currencyCode, currencyRate, foreignAmount, nativeAmount)
            .SetDocNo(docNo)
            .SetDueDate(dueDate)
            .SetProject(project)
            .SetDepartment(department)
            .SetRegion(region)
            .SetCustom1(custom1)
            .SetCustom2(custom2)
            .SetItemQty(itemQty)
            .SetIsOriginal(isOriginal);

        return this;
    }
}