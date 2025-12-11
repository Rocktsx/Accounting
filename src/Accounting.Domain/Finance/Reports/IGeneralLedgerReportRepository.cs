using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    public interface IGeneralLedgerReportRepository
    {
        Task<IEnumerable<GeneralLedgerSingleCurrencyReportResult>> GetGLSingleCurrencyListAsync(
            DateOnly startDate, DateOnly endDate, DateOnly periodStartDate, DateOnly periodEndDate,
            Guid? subjectId = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<GeneralLedgerMultipleCurrencyReportResult>> GetGLMultipleCurrencyListAsync(
           DateOnly startDate, DateOnly endDate, DateOnly periodStartDate, DateOnly periodEndDate,
           Guid? subjectId = null, CancellationToken cancellationToken = default);
    }
}
