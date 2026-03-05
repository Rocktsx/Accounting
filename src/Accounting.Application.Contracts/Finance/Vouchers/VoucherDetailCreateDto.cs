using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Vouchers;

public class VoucherDetailCreateDto
{ 
    [Required] public Guid SubjectId { get; set; }
    public Guid? SubSubjectCode { get; set; }

    [MaxLength(AccountingCommonConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    public DebitorCreditor DebitorCreditor { get; set; }

    [Required]
    [MaxLength(AccountingCommonConsts.MaxCodeLength)]
    public string CurrencyCode { get; set; }

    [Required] public decimal CurrencyRate { get; set; }
    [Required] public decimal ForeignAmount { get; set; }
    [Required] public decimal NativeAmount { get; set; }

    [MaxLength(AccountingCommonConsts.MaxCommonTextFieldLength)]
    public string? DocNo { get; set; }

    public DateTime? DueDate { get; set; }

    [MaxLength(AccountingCommonConsts.MaxCommonTextFieldLength)]
    public string? Project { get; set; }

    [MaxLength(AccountingCommonConsts.MaxCommonTextFieldLength)]
    public string? Department { get; set; }

    [MaxLength(AccountingCommonConsts.MaxCommonTextFieldLength)]
    public string? Region { get; set; }

    [MaxLength(AccountingCommonConsts.MaxCommonTextFieldLength)]
    public string? Custom1 { get; set; }

    [MaxLength(AccountingCommonConsts.MaxCommonTextFieldLength)]
    public string? Custom2 { get; set; }

    public int? ItemQty { get; set; }
    public bool? IsOriginal { get; set; }

    [MaxLength(AccountingCommonConsts.MaxCommonTextFieldLength)]
    public string? PaymentReference { get; set; }

    public VoucherDetailCreateDto()
    {
        CurrencyCode = string.Empty;
    }
}