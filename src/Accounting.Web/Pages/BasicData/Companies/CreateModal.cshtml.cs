using Accounting.BasicData;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.BasicData.Companies
{
    public class CreateModal : PageModel
    {
        public CreateOrEditCompanyViewModel Company { get; set; }

        public List<SelectListItem> Currencies { get; set; }

        private readonly ICurrencyAppService _currencyAppService;

        [BindProperty(SupportsGet = true)]
        public bool IsVendor { get; set; } = false;
        public CreateModal(ICurrencyAppService currencyAppService)
        {
            _currencyAppService = currencyAppService;
        }
        public async Task OnGet()
        {
            Company = new CreateOrEditCompanyViewModel
            {
                IsClient = true,
                IsVendor = false,
                Prefix ="C"
            };
            var currencies =await _currencyAppService.GetActiveListAsync();
             Currencies =currencies.Select(c => new SelectListItem() { Text = c.TargetCurrency, Value = c.TargetCurrency })
                .ToList();

            if(Currencies.Count()> 0)
            {
                Currencies.First().Selected = true;
            }
            if(this.IsVendor)
            {
                Company.Prefix = "V";
                Company.IsClient = false;
                Company.IsVendor = true;
            }
        }
    }
}
