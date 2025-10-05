
namespace Accounting.Finance.Vouchers
{
    public class VoucherUpdateStatusDto
    {
        public VoucherType? VoucherType { get; set; }
        public VoucherStatus? Status { get; set; }
        public string? Code { get; set; }
    }
}
