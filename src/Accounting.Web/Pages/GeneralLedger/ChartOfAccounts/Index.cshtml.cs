using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.GeneralLedger.ChartOfAccounts
{
    [Authorize(Permissions.AccountingPermissions.Subjects.Default)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
