using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Subjects
{
    public class SubjectVoucherSimpleDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public DebitorCreditor DebitorCreditor { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsSubSubjectType { get; set; }
        public bool IsPayMethod { get; set; }
        public string AccountTypeCode { get; set; }

        public SubjectVoucherSimpleDto()
        {
            Code = string.Empty;
            Name = string.Empty;
            OtherName = string.Empty;
            CurrencyCode = string.Empty;
            AccountTypeCode = string.Empty;
        }
    }
}
