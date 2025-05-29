using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.BasicData
{
    public class CompanySearchDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public bool? IsClient { get; set; }
        public bool? IsVendor { get; set; }
    }
}
