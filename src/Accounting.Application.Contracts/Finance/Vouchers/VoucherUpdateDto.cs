using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Vouchers;

public class VoucherUpdateDto
{
    [Required] public DateTime VoucherDate { get; set; } 

    public IEnumerable<VoucherDetailUpdateDto> Details { get; set; } = [];
}