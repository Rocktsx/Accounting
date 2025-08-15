using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accounting.Web.Pages.BasicData.Companies
{
    public class CreateAddressModalModel : PageModel
    {
        public CreateOrEditCompanyAddressViewModel Address; 
        public void OnGet()
        {
            Address = new CreateOrEditCompanyAddressViewModel()
            {
                IsBilling = true
            };
        }
    }
}
