using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.BasicData.Companies
{
    public class ImportModalModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public bool IsVendor { get; set; } = false;
        public void OnGet()
        {
        }
    }
}
