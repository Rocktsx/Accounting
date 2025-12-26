using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.BankReconciliations
{
    public interface IBankReconciliationReportRepository
    {
        Task<IEnumerable<BankReconciliationReportResult>> GetReportListAsync(DateOnly startDate, DateOnly endDate,
            Guid? subjectId = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<BankReconciliationUnpresentedReportResult>> GetUnpresentedReportListAsync(DateOnly startDate, DateOnly endDate,
           Guid? subjectId = null, CancellationToken cancellationToken = default);
    }
}
