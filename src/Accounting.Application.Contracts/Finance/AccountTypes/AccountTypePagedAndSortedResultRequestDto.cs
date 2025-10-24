using Accounting.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.AccountTypes
{
    public class AccountTypePagedAndSortedResultRequestDto : FilteredPagedAndSortedResultRequestDto
    {
        public bool? IsIncludeParent { get; set; }
    }
}
