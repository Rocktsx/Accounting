using Accounting.Finance.AccountingPeriods;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.AccountingPeriods
{
    public class CreateModalModel : AccountingPageModel
    { 
        private readonly IServiceProvider _serviceProvider;
        [BindProperty]
        public CreateAccountingPeriodViewModel Item { get; set; }

        public CreateModalModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Item = new CreateAccountingPeriodViewModel();
        }
        public void OnGet()
        {
            Item = new CreateAccountingPeriodViewModel
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }
        public async Task<IActionResult> OnPost()
        {
            var service = _serviceProvider.GetRequiredService<IAccountingPeriodAppService>();
            var dto = ObjectMapper.Map<CreateAccountingPeriodViewModel, AccountingPeriodCreateDto>(Item);
            await service.CreateAsync(dto);
            return NoContent();
        }
    }
}
