using Accounting.Finance.AccountingPeriods;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.AccountingPeriods
{
    public class EditModalModel : AccountingPageModel
    {
        private readonly IAccountingPeriodAppService _service;

        public EditModalModel(IAccountingPeriodAppService service)
        {
            _service = service;
        }
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        [BindProperty]
        public EditAccountingPeriodViewModel Item { get; set; }
        public async Task OnGet()
        {
            var dto = await _service.GetAsync(Id);
            Item = ObjectMapper.Map<AccountingPeriodDto,EditAccountingPeriodViewModel>(dto);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var dto = ObjectMapper.Map<EditAccountingPeriodViewModel, AccountingPeriodUpdateDto>(Item);
            await _service.UpdateAsync(Item.Id, dto);
            return NoContent();
        }
    }
}
