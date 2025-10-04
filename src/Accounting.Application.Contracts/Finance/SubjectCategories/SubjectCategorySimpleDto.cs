using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.SubjectCategories
{
    public class SubjectCategorySimpleDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
    }
}
