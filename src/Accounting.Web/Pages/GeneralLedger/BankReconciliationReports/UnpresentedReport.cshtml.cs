using Accounting.Finance.AccountingPeriods;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.BankReconciliationReports
{
    public class UnpresentedReportModel : PageModel
    {
        private readonly IAccountingPeriodAppService _accountingPeriodAppService;
        public CurrentAccountingPeriodDto CurrentPeriod;
        public IEnumerable<AccountingPeriodDto> Periods { get; set; }
        public Guid? PeriodId { get; set; }
        public UnpresentedReportModel(IAccountingPeriodAppService accountingPeriodAppService)
        {
            _accountingPeriodAppService = accountingPeriodAppService;
        }
        public async Task OnGet()
        {
            CurrentPeriod = await _accountingPeriodAppService.GetCurrentPeriodAsync();
            var dtos = await _accountingPeriodAppService.GetListAsync(new Dtos.FilteredPagedAndSortedResultRequestDto { });
            var items = dtos.Items.OrderByDescending(item => item.StartDate);
            Periods = items;

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
