using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.AccountingPeriods
{
    public interface IAccountingPeriodRepository: IRepository<AccountingPeriod, Guid>
    {
    }
}
