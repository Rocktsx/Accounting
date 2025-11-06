using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Vouchers
{
    public class PaymentDetailDto
    {
        public Guid SubjectId { get; set; }
        public decimal ForeignAmount { get; set; }

        public DebitorCreditor DebitorCreditor { get; set; }

        public string? CurrencyCode { get; set; }

        public decimal CurrencyRate { get; set; }

        public decimal NativeAmount { get; set; }
        public string? PaymentReference { get; set; }
    }
}
