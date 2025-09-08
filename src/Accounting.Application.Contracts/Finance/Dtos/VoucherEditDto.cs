using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Dtos;

public class VoucherEditDto
{
    [Required]
    public DateOnly VoucherDate { get; set; } 

    public IEnumerable<VoucherDetailCreateDto> Details { get; set; } = [];
}