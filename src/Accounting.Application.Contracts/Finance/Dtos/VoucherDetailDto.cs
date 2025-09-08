using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Dtos;

public class VoucherDetailDto: EntityDto<Guid>
{
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
}