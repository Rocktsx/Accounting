using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.BasicData.Client
{
    public class CreateContactModalModel : PageModel
    {
        public CreateOrEditCompanyContactViewModel Contact = new CreateOrEditCompanyContactViewModel(); 
        public void OnGet()
        {
        }
    }
}
