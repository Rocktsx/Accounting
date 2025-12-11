using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Finance.GeneralLedgerReports
{
    /// <summary>
    /// 总账报表应用服务接口
    /// </summary>
    public interface IGeneralLedgerReportAppService
    {
        Task<IEnumerable<GeneralLedgerSingleCurrencyReportResultDto>>
            GetSingleCurrencyListAsync(GeneralLedgerReportRequestDto input);

        Task<IEnumerable<GeneralLedgerMultipleCurrencyReportResultDto>>
            GetMultipleCurrencyListAsync(GeneralLedgerReportRequestDto input);

        Task<IEnumerable<GeneralLedgerMultipleCurrencyReportResultDto>>
           GetMultipleCurrencyGroupListAsync(GeneralLedgerReportRequestDto input);
    }
}
