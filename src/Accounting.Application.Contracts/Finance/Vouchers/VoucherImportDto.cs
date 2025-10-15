using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Vouchers
{
    public class VoucherImportDto
    {
        public IEnumerable<VoucherImportItemDto> Data { get; set; }
        public VoucherImportType ImportType { get; set; }
        public string? SubjectCode { get; set; }
    }
}
