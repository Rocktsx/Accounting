using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Subjects
{
    public class SubjectSimpleDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }

        public SubjectSimpleDto()
        {
            Code = string.Empty;
            Name = string.Empty;
            OtherName = string.Empty;
        }
    }
}
