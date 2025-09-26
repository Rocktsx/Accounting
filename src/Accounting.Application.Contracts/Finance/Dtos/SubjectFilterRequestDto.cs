using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Dtos
{
    public class SubjectFilterRequestDto: FilteredPagedAndSortedResultRequestDto
    {
        public Guid? SubjectCategoryId { get; set; }
        public Guid[]? SubjectIds { get; set; } 
    }
}
