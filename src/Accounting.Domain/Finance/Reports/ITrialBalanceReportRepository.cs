using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    /// <summary>
    /// 试算表报表仓储接口
    /// </summary>
    public interface ITrialBalanceReportRepository
    {
        Task<IEnumerable<TrialBalanceYearToDateResult>> GetYearToDateListAsync(
            DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default);
    }
}
