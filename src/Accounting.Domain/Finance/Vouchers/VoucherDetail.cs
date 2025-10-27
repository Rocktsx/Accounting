using Accounting.BasicData.Companies;
using Accounting.Finance.Subjects;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance.Vouchers;

public class VoucherDetail : Entity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public Guid VoucherId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid? SubSubjectCode { get; private set; }
    public string Description { get; private set; }
    public DebitorCreditor DebitorCreditor { get; private set; }
    public string CurrencyCode { get; private set; }
    public decimal CurrencyRate { get; private set; }
    public decimal ForeignAmount { get; private set; }
    public decimal NativeAmount { get; private set; }
    public string DocNo { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public string Project { get; private set; }
    public string Department { get; private set; }
    public string Region { get; private set; }
    public string Custom1 { get; private set; }
    public string Custom2 { get; private set; }
    public int ItemQty { get; private set; }
    public bool IsOriginal { get; private set; }
    public string PaymentReference { get; private set; }

    public virtual Voucher? Voucher { get; private set; }
    public virtual Subject? Subject { get; private set; }
    public virtual Company? Company { get; private set; }

    private VoucherDetail()
    {
    } 
    internal VoucherDetail(Guid id, Guid voucherId, Guid subjectId, Guid? subSubjectCode, string description,
        DebitorCreditor debitorCreditor, string currencyCode, decimal currencyRate, decimal foreignAmount, decimal nativeAmount, 
        string docNo, DateOnly? dueDate, int itemQty, bool isOriginal, string paymentReference, Guid? tenantId = null) : base(id)
    {
        SetVoucherId(voucherId);
        SetSubjectId(subjectId);
        SetSubSubjectCode(subSubjectCode);
        SetDescription(description);
        SetDebitorCreditor(debitorCreditor);
        SetCurrencyAndAmount(currencyCode, currencyRate, foreignAmount, nativeAmount);
        SetDocNo(docNo);
        SetDueDate(dueDate);
        SetProject(string.Empty);
        SetDepartment(string.Empty);
        SetRegion(string.Empty);
        SetCustom1(string.Empty);
        SetCustom2(string.Empty);
        SetItemQty(itemQty);
        SetIsOriginal(isOriginal);
        SetPaymentReference(paymentReference);
        TenantId = tenantId;
    }

    public VoucherDetail SetVoucherId(Guid voucherId)
    {
        Check.NotDefaultOrNull<Guid>(voucherId, nameof(voucherId));
        VoucherId = voucherId;
        return this;
    }

    public VoucherDetail SetSubjectId(Guid subjectId)
    {
        if (Guid.Empty.Equals(subjectId))
        {
            throw new BusinessException(AccountingDomainErrorCodes.SubjectIdCanNotBeEmpty);
        }
        SubjectId = subjectId;
        return this;
    }

    public VoucherDetail SetSubSubjectCode(Guid? subSubjectCode)
    {
        SubSubjectCode = subSubjectCode;
        return this;
    }

    public VoucherDetail SetDescription(string description)
    {
        Description = description ?? string.Empty;
        return this;
    }

    public VoucherDetail SetDebitorCreditor(DebitorCreditor debitorCreditor)
    {
        DebitorCreditor = debitorCreditor;
        return this;
    }

    public VoucherDetail SetCurrencyAndAmount(string currencyCode, decimal currencyRate, decimal foreignAmount,
        decimal nativeAmount)
    {
        Check.NotNullOrWhiteSpace(currencyCode, nameof(currencyCode));
        Check.Positive(currencyRate, nameof(currencyRate));
        Check.Positive(foreignAmount, nameof(foreignAmount));
        Check.Positive(nativeAmount, nameof(nativeAmount));
        if (currencyRate * foreignAmount != nativeAmount)
        {
            throw new BusinessException(AccountingDomainErrorCodes.ForeignExchangeRateMatchNativeAmount);
        }
        CurrencyCode = currencyCode;
        CurrencyRate = currencyRate;
        ForeignAmount = foreignAmount;
        NativeAmount = nativeAmount;

        return this;
    }


    public VoucherDetail SetDocNo(string docNo)
    {
        DocNo = docNo ?? string.Empty;
        return this;
    }

    public VoucherDetail SetDueDate(DateOnly? dueDate)
    {
        DueDate = dueDate;
        return this;
    }

    public VoucherDetail SetProject(string project)
    {
        Project = project ?? string.Empty;
        return this;
    }

    public VoucherDetail SetDepartment(string department)
    {
        Department = department ?? string.Empty;
        return this;
    }

    public VoucherDetail SetRegion(string region)
    {
        Region = region ?? string.Empty;
        return this;
    }

    public VoucherDetail SetCustom1(string custom1)
    {
        Custom1 = custom1 ?? string.Empty;
        return this;
    }

    public VoucherDetail SetCustom2(string custom2)
    {
        Custom2 = custom2 ?? string.Empty;
        return this;
    }

    public VoucherDetail SetItemQty(int itemQty)
    {
        ItemQty = itemQty;
        return this;
    }

    public VoucherDetail SetIsOriginal(bool isOriginal)
    {
        IsOriginal = isOriginal;
        return this;
    }
    public VoucherDetail SetPaymentReference(string paymentReference)
    {
        PaymentReference = paymentReference ?? string.Empty;
        return this;
    }
}