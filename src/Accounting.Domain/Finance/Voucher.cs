using System;
using System.Collections.Generic;
using System.Linq; 
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance;

public class Voucher : AuditedAggregateRootWithCode<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public DateOnly VoucherDate { get; private set; }
    public VoucherType VoucherType { get; private set; }
    public VoucherStatus Status { get; private set; }

    public virtual ICollection<VoucherDetail> Details { get; private set; } = [];

    private Voucher()
    {
    }

    public Voucher(Guid id, DateOnly voucherDate, VoucherType voucherType, VoucherStatus status, Guid? tenantId = null)
    {
        Id = id;
        SetVoucherDate(voucherDate);
        SetVoucherType(voucherType);
        SetStatus(status);
        TenantId = tenantId;
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

    public VoucherDetail AddDetail(Guid id, Guid subjectId, Guid? subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docNo, DateOnly? dueDate, int itemQty, bool isOriginal, string paymentReference)
    {
        var item = new VoucherDetail(id, this.Id, subjectId, subSubjectCode, description, debitorCreditor, currencyCode,
            currencyRate, foreignAmount, nativeAmount, docNo, dueDate, itemQty, isOriginal,paymentReference, TenantId);
        Details.Add(item);
        return item;
    }

    public VoucherDetail SetDetail(Guid id, Guid subjectId, Guid? subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docNo, DateOnly? dueDate, int itemQty, bool isOriginal, string paymentReference)
    {
        var item = Details.FirstOrDefault(obj => obj.Id == id);
        if (item == null)
        {
            return null;
        }

        item.SetSubjectId(subjectId)
            .SetDebitorCreditor(debitorCreditor)
            .SetSubSubjectCode(subSubjectCode)
            .SetDescription(description)
            .SetCurrencyAndAmount(currencyCode, currencyRate, foreignAmount, nativeAmount)
            .SetDocNo(docNo)
            .SetDueDate(dueDate)
            .SetItemQty(itemQty)
            .SetIsOriginal(isOriginal)
            .SetPaymentReference(paymentReference);

        return item;
    }
}