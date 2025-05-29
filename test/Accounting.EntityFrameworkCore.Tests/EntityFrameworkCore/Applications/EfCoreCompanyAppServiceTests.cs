using Accounting.BasicData;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace Accounting.EntityFrameworkCore.Applications
{
    [Collection(AccountingTestConsts.CollectionDefinitionName)]
    public class EfCoreCompanyAppServiceTests : CompanyAppServiceTests<AccountingEntityFrameworkCoreTestModule>
    {
          
    }
}
