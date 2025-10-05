using Accounting.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Accounting.EntityFrameworkCore.Applications
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreVoucherStateAppServiceTests : VoucherStateAppServiceTests<AccountingEntityFrameworkCoreTestModule>
    {
    }
}
