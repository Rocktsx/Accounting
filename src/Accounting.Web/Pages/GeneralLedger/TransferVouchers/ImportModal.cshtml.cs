using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.GeneralLedger.TransferVouchers
{
    [Authorize(Permissions.AccountingPermissions.TransferVouchers.Import)]
    public class ImportModalModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
