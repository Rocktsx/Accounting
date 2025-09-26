using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.BasicData.Dtos
{
    public class CompanySearchDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public bool? IsClient { get; set; }
        public bool? IsVendor { get; set; }
        public Guid[]? Ids { get; set; }
    }
}
