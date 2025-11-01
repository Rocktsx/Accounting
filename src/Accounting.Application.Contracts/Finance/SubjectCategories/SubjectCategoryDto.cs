using Accounting.Finance.AccountTypes;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Accounting.Finance.SubjectCategories
{
    public class SubjectCategoryDto: AuditedEntityDto<Guid>, IHasConcurrencyStamp
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

        public string ConcurrencyStamp { get; set; }
    }
}
