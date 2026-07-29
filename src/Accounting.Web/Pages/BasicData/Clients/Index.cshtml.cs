using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.BasicData.Clients
{
    [Authorize(Permissions.AccountingPermissions.Clients.Default)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
