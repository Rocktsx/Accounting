using Accounting.Finance;
using Accounting.Finance.Vouchers;
using AutoMapper;
using Magicodes.ExporterAndImporter.Core;
using System;

namespace Accounting.Models
{
    [AutoMap(typeof(VoucherImportItemDto), ReverseMap = true)]
    public class VoucherImportItemModel
    {
        [ImporterHeader(Name = "Group No.")]
        public int GroupNo { get; set; }

        [ImporterHeader(Name = "Type")]
        [ValueMapping("JV", 0)]
        [ValueMapping("RV", 1)]
        [ValueMapping("PV", 2)]
        public VoucherType? VoucherType { get; set; }

        [ImporterHeader(Name = "Prefix")]
        public string? Prefix { get; set; }

        [ImporterHeader(Name = "Voucher No.")]
        public string? VoucherCode { get; set; }

        [ImporterHeader(Name = "Voucher Date")]
        public DateTime VoucherDate { get; set; }

        [ImporterHeader(Name = "Subject Code")]
        public string? SubjectCode { get; set; }

        [ImporterHeader(Name = "Subject Name")]
        public string? SubjectName { get; set; }

        [ImporterHeader(Name = "Sub Code")]
        public string? SubSubjectCode { get; set; }

        [ImporterHeader(Name = "Sub Name")]
        public string? SubSubjectName { get; set; }

        [ImporterHeader(Name = "Description")]
        public string? Description { get; set; }

        [ImporterHeader(Name = "Currency")]
        public string? Currency { get; set; }

        [ImporterHeader(Name = "Currency Rate")]
        public decimal CurrencyRate { get; set; }

        [ImporterHeader(Name = "Debit")]
        public decimal? Debit { get; set; }

        [ImporterHeader(Name = "Credit")]
        public decimal? Credit { get; set; }

        [ImporterHeader(Name = "Doc. No.")]

        public string? DocNo { get; set; }

        [ImporterHeader(Name = "Payment Ref.")]
        public string? PaymentReference { get; set; }

        [ImporterHeader(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [ImporterHeader(Name = "Project")]
        public string? Project { get; set; }

        [ImporterHeader(Name = "Department")]
        public string? Department { get; set; }

        [ImporterHeader(Name = "Region")]
        public string? Region { get; set; }

        [ImporterHeader(Name = "Custom1")]
        public string? Custom1 { get; set; }

        [ImporterHeader(Name = "Custom2")]
        public string? Custom2 { get; set; }

        [ImporterHeader(Name = "Statement Desc")]
        public string? StatementDesc { get; set; }

        [ImporterHeader(Name = "Item Qty")]
        public int? ItemQty { get; set; }
    }
}
