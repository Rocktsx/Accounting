using Accounting.Finance;
using Accounting.Finance.Vouchers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Accounting.Web.Pages.GeneralLedger.VoucherStates
{
    public class IndexModel : AccountingPageModel
    {
        public List<SelectListItem> VoucherTypes { get; set; }
        public List<SelectListItem> VoucherStates { get; set; } 
      
        public void OnGet()
        {
            VoucherTypes = Helpers.GetEnumSelectList(typeof(VoucherType), L);
            VoucherStates = Helpers.GetEnumSelectList(typeof(VoucherStatus), L);
        }
    }
}
