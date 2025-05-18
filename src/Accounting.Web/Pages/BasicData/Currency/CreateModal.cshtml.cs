using Accounting.BasicData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.BasicData.Currency
{
    public class CreateCurrencyModalModel : AccountingPageModel
    {
        private readonly ICurrencyAppService _currencyAppService;
        [BindProperty]
        public CreateEditCurrencyViewModel Currency { get; set; }

        public CreateCurrencyModalModel(CurrencyAppService currencyAppService)
        {
            _currencyAppService = currencyAppService;
        }
        public void OnGet()
        {
            Currency = new CreateEditCurrencyViewModel() { EffectiveDate = DateOnly.FromDateTime(DateTime.Now) };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _currencyAppService.CreateAsync(ObjectMapper.Map<CreateEditCurrencyViewModel, CurrencyCreateDto>(Currency));
            return NoContent();
        }
    }
}
