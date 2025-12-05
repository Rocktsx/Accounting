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

    bool _enableProjectFunction = false;
    bool _enableRegionFunction = false;
    bool _enableDepartmentFunction = false; 
    bool _enableCustom1Function = false;
    bool _enableCustom2Function = false;

    public virtual ICollection<VoucherDetail> Details { get; private set; } = [];

    private Voucher()
    {
    }

    public Voucher(Guid id, DateOnly voucherDate, VoucherType voucherType, Guid? tenantId = null)
    {
        Id = id;
        SetStatus(VoucherStatus.Draft);
        SetVoucherDate(voucherDate);
        SetVoucherType(voucherType);
        TenantId = tenantId;
    }

    public Voucher SetVoucherDate(DateOnly voucherDate)
    {
        CheckStatus();

        VoucherDate = voucherDate;
        return this;
    }

    public Voucher SetVoucherType(VoucherType voucherType)
    {
        CheckStatus();

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
        decimal nativeAmount, string docNo, DateOnly? dueDate, int itemQty, bool isOriginal, string paymentReference,
        string project = null, string region = null, string department = null, string custom1 = null, string custom2 = null)
    {
        CheckStatus();

        var item = new VoucherDetail(id, Id, subjectId, subSubjectCode, description, debitorCreditor, currencyCode,
            currencyRate, foreignAmount, nativeAmount, docNo, dueDate, itemQty,
            VoucherType == VoucherType.JournalVoucher || isOriginal, paymentReference, TenantId);
        Details.Add(item);
        SetFunctionalFields(item, project, region, department, custom1, custom2);

        return this;
    }

    public Voucher SetDetail(Guid id, Guid subjectId, Guid? subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount, string docNo, DateOnly? dueDate, int itemQty, bool isOriginal, string paymentReference,
        string project = null,  string region = null, string department = null, string custom1 = null, string custom2 = null)
    {
        CheckStatus();

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
            .SetIsOriginal(VoucherType == VoucherType.JournalVoucher || isOriginal)
            .SetPaymentReference(paymentReference);
        SetFunctionalFields(item, project, region, department, custom1, custom2);
        return this;
    }
    private void SetFunctionalFields(VoucherDetail item, string project,
           string region, string department, string custom1, string custom2)
    { 
        item.SetProject(project, _enableProjectFunction);
        item.SetRegion(region, _enableRegionFunction);
        item.SetDepartment(department, _enableDepartmentFunction);
        item.SetCustom1(custom1, _enableCustom1Function);
        item.SetCustom2(custom2, _enableCustom2Function); 
    }
    public void ValidateBalance()
    {
        var amount = Details.Sum(item => item.NativeAmount * (int)item.DebitorCreditor);
        if (amount != 0)
        {
            throw new BusinessException(AccountingDomainErrorCodes.VoucherDoesNotBalance);
        }
    }
    public Voucher SetFunctionEnable(bool enableProjectFunction, bool enableRegionFunction,
           bool enableDepartmentFunction, bool enableCustom1Function,
           bool enableCustom2Function)
    {
        _enableCustom1Function = enableCustom1Function;
        _enableCustom2Function = enableCustom2Function;
        _enableDepartmentFunction = enableDepartmentFunction;
        _enableProjectFunction = enableProjectFunction;
        _enableRegionFunction = enableRegionFunction;

        return this;
    }
    private void CheckStatus()
    {
        if(Status == VoucherStatus.Draft)
        {
            return;
        }
        throw new BusinessException(AccountingDomainErrorCodes.OnlyDraftStatusVoucherCanUpdate);
    }
}