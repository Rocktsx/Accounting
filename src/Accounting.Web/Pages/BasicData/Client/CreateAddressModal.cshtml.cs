using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.BasicData.Client
{
    public class CreateAddressModalModel : PageModel
    {
        public CreateOrEditCompanyAddressViewModel Address = new CreateOrEditCompanyAddressViewModel(); 
        public void OnGet()
        {
        }
    }
}
