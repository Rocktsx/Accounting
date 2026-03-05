using System.Collections.Generic;

namespace Accounting.Finance.Vouchers
{
    public class VoucherImportDto
    {
        public IEnumerable<VoucherImportItemDto> Data { get; set; } = [];
        public VoucherImportType ImportType { get; set; }
        public string? SubjectCode { get; set; }
    }
}
