using Accounting.Finance;
using Accounting.Finance.Dtos;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GenenalLedger.AccountType
{
    public class EditModalModel : AccountingPageModel
    {
        private readonly IAccountTypeAppService _service;
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        [BindProperty]
        public EditAccountTypeViewModel Item { get; set; }
        public List<SelectListItem> AccountTypes { get; set; }
        public EditModalModel(IAccountTypeAppService service)
        {
            _service = service;
        }
        public async Task OnGet()
        {
            var dto =await _service.GetAsync(Id);
            Item = ObjectMapper.Map<AccountTypeDto, EditAccountTypeViewModel>(dto);
            var dtos = await _service.GetSelectListAsync();
            AccountTypes = dtos.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = Helpers.GetText(item.Code, item.Name, item.OtherName),
            }).ToList();
            AccountTypes.Insert(0, new SelectListItem { Value = null, Text = "--" });
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var dto = ObjectMapper.Map<EditAccountTypeViewModel, AccountTypeUpdateDto>(Item);
            await _service.UpdateAsync(Item.Id, dto);
            return NoContent();
        }
    }
}
