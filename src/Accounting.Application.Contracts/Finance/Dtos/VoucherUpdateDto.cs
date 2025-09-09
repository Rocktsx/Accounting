using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Dtos;

public class VoucherUpdateDto
{
    [Required]
    public DateOnly VoucherDate { get; set; } 

    public IEnumerable<VoucherDetailUpdateDto> Details { get; set; } = [];
}