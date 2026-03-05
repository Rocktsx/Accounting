using Accounting.Finance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Accounting.Web.Pages.GeneralLedger.Components
{
    public class UpdateStatusModalModel : AccountingPageModel
    {
        public List<SelectListItem> VoucherStates { get; set; } = [];
        public void OnGet()
        {
            VoucherStates = Helpers.GetEnumSelectList(typeof(VoucherStatus), L);
        }
    }
}
