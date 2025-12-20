using Accounting.Finance.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Finance.ReceivableAgingReports
{
    public interface IReceivableAgingReportAppService
    {
        Task<IEnumerable<AgingSummarySingleCurrencyResultDto>> GetAgingSummarySingleCurrencyListAsync(
            AgingReportRequestDto input);

        Task<IEnumerable<AgingSummaryMultipleCurrencyResultDto>> GetAgingSummaryMultipleCurrencyListAsync(
           AgingReportRequestDto input);

        Task<IEnumerable<AgingDetailResultDto>> GetAgingDetailListAsync(
           AgingReportRequestDto input);
    }
}
