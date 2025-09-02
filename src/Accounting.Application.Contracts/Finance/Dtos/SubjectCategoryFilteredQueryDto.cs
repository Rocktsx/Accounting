using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Dtos
{
    public class SubjectCategoryFilteredQueryDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public Guid? ParentId { get; set; }
        public DebitorCreditor DebitorCreditor { get; set; }
        public Guid? AccountTypeId { get; set; }
        public bool ShowDetail { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public string AccountTypeCode { get; set; }
        public string AccountTypeName { get; set; }
        public string AccountTypeOtherName { get; set; }
        public string ParentCode { get; set; }
        public string ParentName { get; set; }
        public string ParentOtherName { get; set; } 
    }
}
