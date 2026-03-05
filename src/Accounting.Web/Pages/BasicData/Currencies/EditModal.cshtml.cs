using Accounting.BasicData.Currencies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.BasicData.Currencies
{
    public class EditModal : AccountingPageModel
    {
        private readonly ICurrencyAppService _currencyAppService;
        [BindProperty(SupportsGet =true)]
        public Guid Id { get; set; }

       [BindProperty]
        public CreateEditCurrencyViewModel Currency { get; set; }

        public EditModal(ICurrencyAppService currencyAppService)
        {
            _currencyAppService = currencyAppService;
            Currency = new CreateEditCurrencyViewModel();
        }
        public async Task OnGet()
        {
            var dto = await _currencyAppService.GetAsync(Id);
            Currency = ObjectMapper.Map<CurrencyDto, CreateEditCurrencyViewModel>(dto);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _currencyAppService.UpdateAsync(Currency.Id, ObjectMapper.Map<CreateEditCurrencyViewModel, CurrencyUpdateDto>(Currency));
            return NoContent();
        }
    }
}
