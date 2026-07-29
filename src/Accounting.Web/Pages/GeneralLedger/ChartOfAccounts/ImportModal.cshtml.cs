using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.GeneralLedger.ChartOfAccounts
{
    [Authorize(Permissions.AccountingPermissions.Subjects.Import)]
    public class ImportModalModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
