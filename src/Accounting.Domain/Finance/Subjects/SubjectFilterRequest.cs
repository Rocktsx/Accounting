using System;
using System.Collections.Generic;

namespace Accounting.Finance.Subjects
{
    public class SubjectFilterRequest
    {
        public string? Filter { get; set; }
        public Guid? SubjectCategoryId { get; set; }
        public IEnumerable<Guid>? SubjectIds { get; set; }
        public IEnumerable<string?>? Codes { get; set; }
        public bool? IsIncludeAccountType { get; set; }

        public bool? IsPaymentMethod { get; set; }
    }
}
