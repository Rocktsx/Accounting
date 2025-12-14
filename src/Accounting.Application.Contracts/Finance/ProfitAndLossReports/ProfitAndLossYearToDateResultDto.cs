using Accounting.Finance.Reports; 

namespace Accounting.Finance.ProfitAndLossReports
{
    public class ProfitAndLossYearToDateResultDto: ReportGroupBaseResultDto
    {
        public AccountTypeGroup Category { get; set; }
    }
}
