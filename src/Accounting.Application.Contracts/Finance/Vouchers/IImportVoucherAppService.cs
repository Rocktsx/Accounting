using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Vouchers
{
    public interface IImportVoucherAppService
    {
        Task<int> ImportDataAsync(VoucherImportDto input);
    }
}
