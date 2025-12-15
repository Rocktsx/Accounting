using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    /// <summary>
    /// 损益表报表存储接口
    /// </summary>
    public interface IProfitAndLossReportRepository
    {
        Task<IEnumerable<ProfitAndLossYearToDateResult>> GetYearToDateListAsync(
           DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default);

        Task<IEnumerable<ProfitAndLossMonthToDateYearToDateResult>> GetMonthToDateAndYearToDateListAsync(DateOnly startDate,
            DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default);

        Task<IEnumerable<ProfitAndLoss12MonthsResult>> Get12MonthsListAsync(DateOnly startDate,
            DateOnly endDate, DateOnly periodStartDate, CancellationToken cancellationToken = default);
    }
}
