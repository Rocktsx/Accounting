
namespace Accounting.Finance.Reports
{
    /// <summary>
    /// Month To Date and Year To Date group base result dto
    /// </summary>
    public class ReportGroupBaseMtdYtdResultDto: ReportGroupBaseResultDto
    {
        public decimal MonthToDateNativeAmount { get; set; }
        public decimal LastPeriodNativeAmount { get; set; }
    }
}
