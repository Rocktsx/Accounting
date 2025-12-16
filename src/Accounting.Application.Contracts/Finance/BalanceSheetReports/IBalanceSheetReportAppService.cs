
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Finance.BalanceSheetReports
{
    public interface IBalanceSheetReportAppService
    {
        /// <summary>
        ///  获取年初至今列表（Year To Date List）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<BalanceSheetYearToDateResultDto>> GetYtdListAsync(
           BalanceSheetYearToDateRequestDto input);

        /// <summary>
        /// 获取月初至今和年初至今列表（Month To Date and Year To Date)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<BalanceSheetMonthToDateYearToDateResultDto>> GetMtdYtdListAsync(
            BalanceSheetMtdYtdRequestDto input);
    }
}
