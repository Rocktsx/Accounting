using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Vouchers;

public class VoucherDetailDto: EntityDto<Guid>
{
    public Guid VoucherId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? SubSubjectCode { get; set; }
    public string Description { get; set; }
    public DebitorCreditor DebitorCreditor { get; set; }
    public string CurrencyCode { get; set; }
    public decimal CurrencyRate { get; set; }
    public decimal ForeignAmount { get; set; }
    public decimal NativeAmount { get; set; } 
    public string DocNo { get; set; }
    public DateOnly? DueDate { get; set; }
    public string Project { get; set; }
    public string Department { get; set; }
    public string Region { get; set; }
    public string Custom1 { get; set; }
    public string Custom2 { get; set; }
    public int ItemQty { get; set; }
    public bool IsOriginal { get; set; }
    public string PaymentReference { get; set; }
}