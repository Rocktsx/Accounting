using System;

namespace Accounting.Finance.Dtos;

public class VoucherFilterRequestDto : FilteredPagedAndSortedResultRequestDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public VoucherType? VoucherType { get; set; }
    public VoucherStatus? Status { get; set; }
    public string? Prefix { get; set; }
    public int? StartNo { get; set; }
    public int? EndNo { get; set; }
    public string? DocNo { get; set; }
}