using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.BasicData.Currencies
{
    [Authorize(Permissions.AccountingPermissions.Currencies.Default)]
    public class IndexModel : AccountingPageModel
    {
        public void OnGet()
        { 
        }
    }
}
