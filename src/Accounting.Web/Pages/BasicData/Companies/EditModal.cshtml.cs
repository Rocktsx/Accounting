using Accounting.BasicData;
using Accounting.BasicData.Dtos;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.BasicData.Companies
{
    public class EditModalModel : AccountingPageModel
    {
        [BindProperty(SupportsGet =true)]
        public Guid Id { get; set; }
        public CreateOrEditCompanyViewModel Company { get; set; }

        public List<SelectListItem> Currencies { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool IsVendor { get; set; } = false;

        private readonly ICurrencyAppService _currencyAppService;
        private readonly ICompanyAppService? _companyAppService;
        public EditModalModel(ICurrencyAppService currencyAppService, IServiceProvider serviceProvider)
        {
            _currencyAppService = currencyAppService;
            _companyAppService = (ICompanyAppService?)serviceProvider.GetService(IsVendor ? typeof(IVendorAppService):typeof( IClientAppService));
        }
        public async Task OnGet()
        {
            var item = await _companyAppService?.GetAsync(Id);
            Company =  ObjectMapper.Map<CompanyDto, CreateOrEditCompanyViewModel> (item);
            var currencies = await _currencyAppService.GetActiveListAsync();
            Currencies = currencies.Select(c => new SelectListItem() { Text = c.TargetCurrency, Value = c.TargetCurrency })
               .ToList(); 
        }
    }
}
