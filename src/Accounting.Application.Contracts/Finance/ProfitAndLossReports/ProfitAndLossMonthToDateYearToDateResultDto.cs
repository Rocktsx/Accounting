using Accounting.Finance.Reports;

namespace Accounting.Finance.ProfitAndLossReports
{
    public class ProfitAndLossMonthToDateYearToDateResultDto: ReportGroupBaseMtdYtdResultDto
    {
        public AccountTypeGroup Category { get; set; }
    }
}
