using Accounting.BasicData;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Accounting.Web.Pages.BasicData.Client
{
    public class EditModalModel : AccountingPageModel
    {
        [BindProperty(SupportsGet =true)]
        public Guid Id { get; set; }
        public CreateOrEditCompanyViewModel Client { get; set; }

        public List<SelectListItem> Currencies { get; set; }

        private readonly ICurrencyAppService _currencyAppService;
        private readonly ICompanyAppService _companyAppService;
        public EditModalModel(ICurrencyAppService currencyAppService, ICompanyAppService companyAppService)
        {
            _currencyAppService = currencyAppService;
            _companyAppService = companyAppService;
        }
        public async Task OnGet()
        {
            var item = await _companyAppService.GetAsync(Id);
            Client =  ObjectMapper.Map<CompanyDto, CreateOrEditCompanyViewModel> (item);
            var currencies = await _currencyAppService.GetActiveListAsync();
            Currencies = currencies.Select(c => new SelectListItem() { Text = c.TargetCurrency, Value = c.TargetCurrency })
               .ToList(); 
        }
    }
}
