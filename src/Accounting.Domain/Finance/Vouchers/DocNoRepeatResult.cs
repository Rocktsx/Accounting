using System;

namespace Accounting.Finance.Vouchers
{
    public class DocNoRepeatResult
    {
        public string DocNo { get; set; }
        public Guid? SubSubjectCode { get; set; }
        public string AccountTypeCode { get; set; }
        public AccountTypeTypes Category { get; set; }
        public int Count { get; set; }
    }
}
