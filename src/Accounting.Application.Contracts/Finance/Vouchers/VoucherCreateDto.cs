using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Vouchers;

public class VoucherCreateDto
{
    [Required]
    [MaxLength(AccountingCommonConsts.MaxPrefixLength)]
    public string Prefix { get; set; }

    public int? GenNo { get; set; }
    [Required]
    public DateTime VoucherDate { get; set; }
    [Required]
    public VoucherType VoucherType { get; set; } 

    public IEnumerable<VoucherDetailCreateDto> Details { get; set; } = [];

    public VoucherCreateDto()
    {
        Prefix = string.Empty;
    }
}