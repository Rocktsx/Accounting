using Accounting.Finance.AccountTypes;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.SubjectCategories
{
    public class CreateModalModel : AccountingPageModel
    {
        private IAccountTypeAppService _service; 
        [BindProperty]
        public CreateAccountTypeViewModel Item { get; set; }
        public List<SelectListItem> AccountTypes { get; set; } 
        public CreateModalModel(IAccountTypeAppService service)
        {
            _service = service;
        }
        public async Task OnGet()
        {
            Item = new CreateAccountTypeViewModel();
            var dtos = await _service.GetSimpleListAsync(); 
            AccountTypes = dtos.ToSelectListItems(
                item => item.Id.ToString(),
                item => Helpers.GetText(item.Code, item.Name, item.OtherName));
        }
        public async Task<IActionResult> OnPost()
        {  
            var dto = ObjectMapper.Map<CreateAccountTypeViewModel, AccountTypeCreateDto>(Item);
            await _service.CreateAsync(dto);
            return NoContent();
        }
    }
}
