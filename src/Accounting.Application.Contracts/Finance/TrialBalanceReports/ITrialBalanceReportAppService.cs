using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Finance.TrialBalanceReports
{
    public interface ITrialBalanceReportAppService
    {
        /// <summary>
        ///  获取年初至今列表（Year To Date List）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<TrialBalanceYearToDateResultDto>> GetYtdListAsync(
           TrialBalanceYtdRequestDto input);

        /// <summary>
        /// 获取月初至今和年初至今列表（Month To Date and Year To Date)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<TrialBalanceMonthToDateYearToDateResultDto>> GetMtdYtdListAsync(
            TrialBalanceMtdYtdRequestDto input);
    }
}
