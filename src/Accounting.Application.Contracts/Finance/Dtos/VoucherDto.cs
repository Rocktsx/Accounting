using System;
using System.Collections.Generic;

namespace Accounting.Finance.Dtos;

public class VoucherDto  : GenerateCodeDto
{
    public Guid Id { get; set; }
    public DateOnly VoucherDate { get; set; }
    public VoucherType VoucherType { get; set; }
    public VoucherStatus Status { get; set; }

    public IEnumerable<VoucherDetailDto> Details { get; set; } = [];
}