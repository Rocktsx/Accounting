using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.GeneralLedger.SubjectCategories
{
    [Authorize(Permissions.AccountingPermissions.SubjectCategories.Default)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
