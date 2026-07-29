using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.Payable.CreditorAgingReports
{
    [Authorize(Permissions.AccountingPermissions.PayableAgingReports.AgingSummarySingleCurrency)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
