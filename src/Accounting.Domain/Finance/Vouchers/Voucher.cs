using Accounting.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance.Vouchers;

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

    public Voucher AddDetail(Guid id, Guid subjectId, Guid? subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docNo, DateOnly? dueDate, int itemQty, bool isOriginal, string paymentReference)
    {
        var item = new VoucherDetail(id, Id, subjectId, subSubjectCode, description, debitorCreditor, currencyCode,
            currencyRate, foreignAmount, nativeAmount, docNo, dueDate, itemQty, isOriginal, paymentReference, TenantId);
        Details.Add(item);
        return this;
    }

    public Voucher SetDetail(Guid id, Guid subjectId, Guid? subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docNo, DateOnly? dueDate, int itemQty, bool isOriginal, string paymentReference)
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
            .SetItemQty(itemQty)
            .SetIsOriginal(isOriginal)
            .SetPaymentReference(paymentReference);

        return this;
    }
    public Voucher SetFunctionalFields(Guid detailId,
           bool enableProjectFunction, bool enableRegionFunction,
           bool enableDepartmentFunction, bool enableCustom1Function,
           bool enableCustom2Function, string project,
           string region, string department, string custom1, string custom2)
    {
        var item = Details.FirstOrDefault(obj => obj.Id == detailId);
        if (item == null)
        {
            return this;
        }
        item.SetProject(project, enableProjectFunction);
        item.SetRegion(region, enableRegionFunction);
        item.SetDepartment(department, enableDepartmentFunction);
        item.SetCustom1(custom1, enableCustom1Function);
        item.SetCustom2(custom2, enableCustom2Function);

        return this;
    }
    public void ValidateBalance()
    {
        var amount = Details.Sum(item => item.NativeAmount * (int)item.DebitorCreditor);
        if (amount != 0)
        {
            throw new BusinessException(AccountingDomainErrorCodes.VoucherDoesNotBalance);
        }
    }
}