using Accounting.Finance.AccountTypes;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.SubjectCategories
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
            var dtos = await _service.GetSimpleListAsync();
            AccountTypes = dtos.ToSelectListItems(
               item => item.Id.ToString(),
               item => Helpers.GetText(item.Code, item.Name, item.OtherName));
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var dto = ObjectMapper.Map<EditAccountTypeViewModel, AccountTypeUpdateDto>(Item);
            await _service.UpdateAsync(Item.Id, dto);
            return NoContent();
        }
    }
}
