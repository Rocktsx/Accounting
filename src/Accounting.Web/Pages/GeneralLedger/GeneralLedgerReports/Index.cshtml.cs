using Accounting.Finance.AccountingPeriods;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounting.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Accounting.Web.Pages.GeneralLedger.GeneralLedgerReports
{
    public class IndexModel : PageModel
    {
        private readonly IAccountingPeriodAppService _accountingPeriodAppService;
        public CurrentAccountingPeriodDto CurrentPeriod;
        public IEnumerable<SelectListItem> Periods { get; set; }
        public Guid? PeriodId { get; set; }
        public IndexModel(IAccountingPeriodAppService accountingPeriodAppService)
        {
            _accountingPeriodAppService = accountingPeriodAppService;
        }
        public async Task OnGet()
        {
            CurrentPeriod = await _accountingPeriodAppService.GetCurrentPeriodAsync();
            var dtos = await _accountingPeriodAppService.GetListAsync(new Dtos.FilteredPagedAndSortedResultRequestDto { });
            var items = dtos.Items.OrderByDescending(item => item.StartDate);
          
            Periods = items.ToSelectListItems(
                 item => item.Id.ToString(),
                 item => item.Code, false);

            if (items.Count() > 0)
            {
                var first = items.First();
                PeriodId = first.Id;
                CurrentPeriod = new CurrentAccountingPeriodDto
                {
                    StartDate = first.StartDate,
                    EndDate = first.EndDate
                };
            }
        }

    }
}
