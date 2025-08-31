using System; 
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Dtos
{
    public class SubjectCategoryDto: EntityDto<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public Guid? ParentId { get; set; }
        public DebitorCreditor DebitorCreditor { get; set; }
        public Guid? AccountTypeId { get; set; }
        public bool ShowDetail { get; set; }
        public string Description { get; set; }
    }
}
