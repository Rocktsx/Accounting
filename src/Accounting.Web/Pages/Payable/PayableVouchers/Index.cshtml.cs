using Accounting.Finance.AccountingPeriods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.Payable.PayableVouchers
{
    public class IndexModel : PageModel
    {
        private readonly IAccountingPeriodAppService _accountingPeriodAppService;
        public CurrentAccountingPeriodDto CurrentPeriod;
        public IndexModel(IAccountingPeriodAppService accountingPeriodAppService)
        {
            _accountingPeriodAppService = accountingPeriodAppService;
        }
        public async Task OnGet()
        {
            CurrentPeriod = await _accountingPeriodAppService.GetCurrentPeriodAsync();
        }
    }
}
