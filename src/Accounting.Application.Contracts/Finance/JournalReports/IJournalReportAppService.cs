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
        Task<IEnumerable<VoucherDto>> GetSingleCurrencySortByCodeListAsync(JournalReportRequestDto input);
        Task<IEnumerable<VoucherDto>> GetSingleCurrencySortByDateListAsync(JournalReportRequestDto input);
        Task<IEnumerable<VoucherDto>> GetMultipleCurrencySortByCodeListAsync(JournalReportRequestDto input);
        Task<IEnumerable<VoucherDto>> GetMultipleCurrencySortByDateListAsync(JournalReportRequestDto input);
    }
}
