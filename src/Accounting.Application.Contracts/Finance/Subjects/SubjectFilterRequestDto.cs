using Accounting.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Subjects
{
    public class SubjectFilterRequestDto : FilteredPagedAndSortedResultRequestDto
    {
        public Guid? SubjectCategoryId { get; set; }
        public Guid[]? SubjectIds { get; set; }
        public bool? IsIncludeAccountType { get; set; }

        public bool? IsPaymentMethod { get; set; }
        /// <summary>
        /// 是不是包含设置的默认应收科目
        /// </summary>
        public bool? IsIncludeReceivableSubject { get; set; }
        /// <summary>
        ///  是不是包含设置的默认应付科目
        /// </summary>
        public bool? IsIncludePayableSubject { get; set; }

        public AccountTypeTypes? AccountTypeCategory { get; set; }
        public void AddSubjectId(string subjectId)
        {
            if (!string.IsNullOrWhiteSpace(subjectId) &&
                Guid.TryParse(subjectId, out var guidId))
            {
                SubjectIds = [.. (SubjectIds ?? []), guidId];
            }
        }
    }
}
