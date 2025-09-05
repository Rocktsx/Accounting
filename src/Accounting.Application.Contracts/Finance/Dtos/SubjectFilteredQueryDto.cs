using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.Dtos
{
    public class SubjectFilteredQueryDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public Guid? SubjectCategoryId { get; set; }
        public Guid? AccountTypeId { get; set; }
        public DebitorCreditor DebitorCreditor { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public bool IsSubSubjectType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPayMethod { get; set; }
        public int? SeqCode { get; set; }
        public string AccountTypeCode { get; set; }
        public string AccountTypeName { get; set; }
        public string AccountTypeOtherName { get; set; }
    }
}
