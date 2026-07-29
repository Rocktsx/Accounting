using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.GeneralLedger.AccountingPeriods
{
    [Authorize(Permissions.AccountingPermissions.AccountingPeriods.Default)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
