using Accounting.Finance.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Finance.ReceivableAgingReports
{
    public interface IReceivableAgingReportAppService
    {
        Task<IEnumerable<AgingSummarySingleCurrencyResultDto>> GetAgingSummarySingleCurrencyListAsync(
            AgingSummarySingleCurrencyRequestDto input);

        Task<IEnumerable<AgingSummaryMultipleCurrencyResultDto>> GetAgingSummaryMultipleCurrencyListAsync(
           AgingSummarySingleCurrencyRequestDto input);
    }
}
