using Accounting.Finance;
using Accounting.Finance.AccountingPeriods;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.Journals
{
    public class MultipleCurrencySortByDateReportModel : AccountingPageModel
    {
        private readonly IAccountingPeriodAppService _accountingPeriodAppService;
        public CurrentAccountingPeriodDto CurrentPeriod;
        public List<SelectListItem> VoucherTypes { get; set; }
        public MultipleCurrencySortByDateReportModel(IAccountingPeriodAppService accountingPeriodAppService)
        {
            _accountingPeriodAppService = accountingPeriodAppService;
        }
        public async Task OnGet()
        {
            CurrentPeriod = await _accountingPeriodAppService.GetCurrentPeriodAsync();
            VoucherTypes = Helpers.GetEnumSelectList(typeof(VoucherType), L, true);
        }
    }
}
