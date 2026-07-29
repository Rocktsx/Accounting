using Accounting.Finance;
using Accounting.Finance.AccountingPeriods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.Journals
{
    [Authorize(Permissions.AccountingPermissions.JournalReports.SingleCurrencySortByCodeReport)]
    public class IndexModel : AccountingPageModel
    {
        private readonly IAccountingPeriodAppService _accountingPeriodAppService;
        public CurrentAccountingPeriodDto CurrentPeriod;
        public List<SelectListItem> VoucherTypes { get; set; }
        public IndexModel(IAccountingPeriodAppService accountingPeriodAppService)
        {
            _accountingPeriodAppService = accountingPeriodAppService;
            CurrentPeriod = new CurrentAccountingPeriodDto();
            VoucherTypes = [];
        }
        public async Task OnGet()
        {
            CurrentPeriod = await _accountingPeriodAppService.GetCurrentPeriodAsync();
            VoucherTypes = Helpers.GetEnumSelectList(typeof(VoucherType), L, true);
        }
    }
}
