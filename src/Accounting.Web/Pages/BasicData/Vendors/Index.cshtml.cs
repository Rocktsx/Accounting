using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.BasicData.Vendors
{
    [Authorize(Permissions.AccountingPermissions.Vendors.Default)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
