using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    /// <summary>
    /// 资产负债表报表仓储接口
    /// </summary>
    public interface IBalanceSheetReportRepository
    {
        Task<IEnumerable<BalanceSheetYearToDateResult>> GetYearToDateListAsync(
            DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default);

        Task<IEnumerable<BalanceSheetMonthToDateYearToDateResult>> GetMonthToDateAndYearToDateListAsync(DateOnly startDate,
            DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default);
    }
}
