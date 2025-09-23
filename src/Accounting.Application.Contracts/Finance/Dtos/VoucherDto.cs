using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Dtos;

public class VoucherDto  : AuditedEntityDto<Guid>
{
    public string Code { get; set; } 
    public string Prefix { get; set; }
    public int GenNo { get; set; } 
    public DateOnly VoucherDate { get; set; }
    public VoucherType VoucherType { get; set; }
    public VoucherStatus Status { get; set; }

    public IEnumerable<VoucherDetailDto> Details { get; set; } = [];
}