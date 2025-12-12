using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    public interface IJournalReportRepository
    {
        Task<IEnumerable<JournalReportSingleCurrencyResult>> GetJLSingleCurrencyListAsync(
            JournalReportRequest request, string? sorting = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<JournalReportMultipleCurrencyResult>> GetJLMultipleCurrencyListAsync(
           JournalReportRequest request, string? sorting = null, CancellationToken cancellationToken = default);
    }
}
