using System;

namespace Accounting.Finance.PayableVouchers
{
    public class PayableDetailDto
    {
        public Guid SourceId { get; set; }

        public Guid SubjectId { get; set; }

        public string DocNo { get; set; }

        public DateOnly VoucherDate { get; set; }

        public decimal ForeignAmount { get; set; }

        public DebitorCreditor DebitorCreditor { get; set; }

        public string CurrencyCode { get; set; }

        public decimal CurrencyRate { get; set; }

        public decimal NativeAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal PaidNativeAmount { get; set; }

        public decimal CurrentPaid { get; set; }

        public decimal NativeCurrentPaid { get; set; }

        public Guid? SubjectCategoryCode { get; set; }

        public string AccType { get; set; }

        public decimal OsAmount { get; set; }

        public DateOnly? DueDate { get; set; }
        public AccountTypeTypes AccTypeCategory { get; set; }
    }
}
