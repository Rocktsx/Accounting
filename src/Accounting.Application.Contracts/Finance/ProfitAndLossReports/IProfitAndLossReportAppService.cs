using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.ProfitAndLossReports
{
    public interface IProfitAndLossReportAppService
    {
        /// <summary>
        ///  获取年初至今列表（Year To Date List）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<ProfitAndLossYearToDateResultDto>> GetYtdListAsync(
           ProfitAndLossYearToDateRequestDto input);

        /// <summary>
        /// 获取月初至今和年初至今列表（Month To Date and Year To Date)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<ProfitAndLossMonthToDateYearToDateResultDto>> GetMtdYtdListAsync(
            ProfitAndLossMtdYtdRequestDto input);

        /// <summary>
        /// 12 Months List
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<ProfitAndLoss12MonthsResultDto>> GetTwelveMonthsListAsync(
            ProfitAndLossMtdYtdRequestDto input); 
    }
}
