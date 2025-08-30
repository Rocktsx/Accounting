using Accounting.Finance;
using Accounting.Finance.Dtos;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GenenalLedger.AccountingPeriod
{
    public class CreateModalModel : AccountingPageModel
    {
        private IAccountingPeriodAppService _service;
        private readonly IServiceProvider _serviceProvider;
        [BindProperty]
        public CreateAccountingPeriodViewModel Item { get; set; }

        public CreateModalModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
            _service = _serviceProvider.GetRequiredService<IAccountingPeriodAppService>();
            var dto = ObjectMapper.Map<CreateAccountingPeriodViewModel, AccountingPeriodCreateDto>(Item);
            await _service.CreateAsync(dto);
            return NoContent();
        }
    }
}
