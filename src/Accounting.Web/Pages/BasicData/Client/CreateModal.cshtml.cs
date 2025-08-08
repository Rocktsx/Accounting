using Accounting.BasicData;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.BasicData.Client
{
    public class CreateModal : PageModel
    {
        public CreateCompanyViewModel Client { get; set; }

        public List<SelectListItem> Currencies { get; set; }

        private readonly ICurrencyAppService _currencyAppService;
        public CreateModal(ICurrencyAppService currencyAppService)
        {
            _currencyAppService = currencyAppService;
        }
        public async Task OnGet()
        {
            Client = new CreateCompanyViewModel
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
        }
    }
}
