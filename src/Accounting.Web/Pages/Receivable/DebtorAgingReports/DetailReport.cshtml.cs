using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.Receivable.DebtorAgingReports
{

    [Authorize(Permissions.AccountingPermissions.ReceivableAgingReports.AgingDetail)]
    public class DetailReportModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
