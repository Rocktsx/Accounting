using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.Receivable.DebtorAgingReports
{
    [Authorize(Permissions.AccountingPermissions.ReceivableAgingReports.AgingSummaryMultipleCurrency)]
    public class SummaryMultipleCurrencyReportModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
