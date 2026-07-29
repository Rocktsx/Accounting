using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.GeneralLedger.GeneralAccounts
{
    [Authorize(Permissions.AccountingPermissions.GeneralAccounts.Default)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
