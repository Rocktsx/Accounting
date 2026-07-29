using Accounting.Finance.AccountingPeriods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.BankReconciliations
{
    [Authorize(Permissions.AccountingPermissions.BankReconciliations.Default)]
    public class IndexModel : PageModel
    {
        private readonly IAccountingPeriodAppService _accountingPeriodAppService;
        public CurrentAccountingPeriodDto CurrentPeriod;
        public IndexModel(IAccountingPeriodAppService accountingPeriodAppService)
        {
            _accountingPeriodAppService = accountingPeriodAppService;
            CurrentPeriod = new CurrentAccountingPeriodDto();
        }
        public async Task OnGet()
        {
            CurrentPeriod = await _accountingPeriodAppService.GetCurrentPeriodAsync();
        }
    }
}
