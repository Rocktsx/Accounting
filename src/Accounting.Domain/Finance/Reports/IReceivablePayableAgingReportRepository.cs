using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    public interface IReceivablePayableAgingReportRepository
    {
        Task<IEnumerable<AgingSummarySingleCurrencyResult>> GetAgingSummarySingleCurrencyListAsync(Guid? subSubjectCode,
            DateOnly endDate, int agingDays = 7, AccountTypeTypes category = AccountTypeTypes.Receivable, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<AgingSummaryMultipleCurrencyResult>> GetAgingSummaryMultipleCurrencyListAsync(Guid? subSubjectCode,
           DateOnly endDate, int agingDays = 7, AccountTypeTypes category = AccountTypeTypes.Receivable,
           CancellationToken cancellationToken = default);

        Task<IEnumerable<AgingDetailResult>> GetAgingDetailListAsync(Guid? subSubjectCode,
          DateOnly endDate, AccountTypeTypes category = AccountTypeTypes.Receivable,
          CancellationToken cancellationToken = default);
    }
}
