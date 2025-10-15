using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Vouchers
{
    public class VoucherImportItemDto
    {
        public int GroupNo { get; set; }
        public VoucherType? VoucherType { get; set; }
        public string? Prefix { get; set; }
        public string? VoucherCode { get; set; }
        public DateTime VoucherDate { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? SubSubjectCode { get; set; }
        public string? SubSubjectName { get; set; }
        public string? Description { get; set; }
        public string? Currency { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal ForeignAmount { get; set; }

        public string? DocNo { get; set; }
        public string? PaymentReference { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Project { get; set; }
        public string? Department { get; set; }
        public string? Region { get; set; }
        public string? Custom1 { get; set; }
        public string? Custom2 { get; set; }
        public string? StatementDesc { get; set; }
        public int ItemQty { get; set; }
    }
}
