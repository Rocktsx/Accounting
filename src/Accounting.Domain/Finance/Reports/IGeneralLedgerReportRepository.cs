using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    public interface IGeneralLedgerReportRepository
    {
        Task<IEnumerable<GeneralLedgerSingleCurrencyReportResult>> GetSingleCurrencyListAsync(
            DateOnly startDate, DateOnly endDate, DateOnly periodStartDate, DateOnly periodEndDate,
            Guid? subjectId = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<GeneralLedgerMultipleCurrencyReportResult>> GetMultipleCurrencyListAsync(
           DateOnly startDate, DateOnly endDate, DateOnly periodStartDate, DateOnly periodEndDate,
           Guid? subjectId = null, bool isGroup = false, CancellationToken cancellationToken = default);
    }
}
