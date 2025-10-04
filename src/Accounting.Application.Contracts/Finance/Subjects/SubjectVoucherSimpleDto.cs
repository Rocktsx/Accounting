using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
