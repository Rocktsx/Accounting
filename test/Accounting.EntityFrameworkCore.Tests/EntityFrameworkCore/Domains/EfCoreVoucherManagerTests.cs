using Accounting.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Accounting.EntityFrameworkCore.Domains
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreVoucherManagerTests: VoucherManagerTests<AccountingEntityFrameworkCoreTestModule>
    {
    }
}
