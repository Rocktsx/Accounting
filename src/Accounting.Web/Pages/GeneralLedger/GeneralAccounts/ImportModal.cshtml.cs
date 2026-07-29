using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.GeneralLedger.GeneralAccounts
{
    [Authorize(Permissions.AccountingPermissions.GeneralAccounts.Import)]
    public class ImportModalModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
