using Accounting.Finance.Vouchers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Finance.JournalReports
{
    /// <summary>
    /// 日志帐报表应用服务接口
    /// </summary>
    public interface IJournalReportAppService
    {
        Task<IEnumerable<JournalSingleCurrencyReportResultDto>> GetSingleCurrencySortByCodeListAsync(
            JournalReportRequestDto input);
        Task<IEnumerable<JournalSingleCurrencyReportResultDto>> GetSingleCurrencySortByDateListAsync(
            JournalReportRequestDto input);
        Task<IEnumerable<JournalMultipleCurrencyReportResultDto>> GetMultipleCurrencySortByCodeListAsync(
            JournalReportRequestDto input);
        Task<IEnumerable<JournalMultipleCurrencyReportResultDto>> GetMultipleCurrencySortByDateListAsync(
            JournalReportRequestDto input);
    }
}
