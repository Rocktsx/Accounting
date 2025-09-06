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
    public DateOnly? DueDate { get; private set; }
    public VoucherType VoucherType { get; private set; }
    public VoucherStatus Status { get; private set; }
    public string Description { get; private set; }
    public string DocType { get; private set; }

    public virtual ICollection<VoucherDetail> Details { get; private set; }

    private Voucher()
    {
    }

    public Voucher(Guid id, DateOnly voucherDate, DateOnly? dueDate, VoucherType voucherType,
        VoucherStatus status, string description, string docType)
    {
        Id = id;
        SetVoucherDate(voucherDate);
        SetDueDate(dueDate);
        SetVoucherType(voucherType);
        SetStatus(status);
        SetDescription(description);
        SetDocType(docType);
    }

    public Voucher SetVoucherDate(DateOnly voucherDate)
    {
        Check.NotNull(voucherDate, nameof(voucherDate));
        VoucherDate = voucherDate;
        return this;
    }

    public Voucher SetDueDate(DateOnly? dueDate)
    {
        DueDate = dueDate;
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

    public Voucher SetDescription(string description)
    {
        Description = description ?? string.Empty;
        return this;
    }

    public Voucher SetDocType(string docType)
    {
        DocType = docType ?? string.Empty;
        return this;
    }

    public Voucher AddDetail(Guid id, Guid subjectId, string subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docType, string docNo, DateOnly? dueDate, string project, string department,
        string region, string custom1, string custom2, decimal itemQty, bool isOriginal)
    {
        var item = new VoucherDetail(id, this.Id, subjectId, subSubjectCode, description, debitorCreditor, currencyCode,
            currencyRate, foreignAmount, nativeAmount, docType, docNo, dueDate, project, department, region, custom1,
            custom2, itemQty, isOriginal);
        Details.Add(item);
        return this;
    }

    public Voucher SetDetail(Guid id, Guid subjectId, string subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docType, string docNo, DateOnly? dueDate, string project, string department,
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
            .SetDocType(docType)
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