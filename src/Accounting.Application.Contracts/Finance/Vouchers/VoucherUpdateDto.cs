using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Accounting.Finance.Vouchers;

public class VoucherUpdateDto: IHasConcurrencyStamp
{
    [Required] public DateTime VoucherDate { get; set; }
    public string ConcurrencyStamp { get; set; }
    public IEnumerable<VoucherDetailUpdateDto> Details { get; set; } = [];
}