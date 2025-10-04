using Accounting.Finance.AccountTypes;
using Accounting.Finance.SubjectCategories;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.GeneralAccount
{
    public class EditModalModel : AccountingPageModel
    {
        private readonly ISubjectCategoryAppService _service;
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        [BindProperty]
        public EditSubjectCategoryViewModel Item { get; set; }
        public List<SelectListItem> AccountTypes { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public EditModalModel(ISubjectCategoryAppService service)
        {
            _service = service;
        }
        public async Task OnGet()
        {
            var dto = await _service.GetAsync(Id);
            Item = ObjectMapper.Map<SubjectCategoryDto, EditSubjectCategoryViewModel>(dto);
            var accountTypeService = LazyServiceProvider.GetRequiredService<IAccountTypeAppService>();
            var dtos = await accountTypeService.GetSimpleListAsync();
            AccountTypes = dtos.ToSelectListItems(
                 item => item.Id.ToString(),
                 item => Helpers.GetText(item.Code, item.Name, item.OtherName));
            var categoryDtos = await _service.GetSimpleListAsync();
            Categories = categoryDtos.ToSelectListItems(
                 item => item.Id.ToString(),
                 item => Helpers.GetText(item.Code, item.Name, item.OtherName));
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var dto = ObjectMapper.Map<EditSubjectCategoryViewModel, SubjectCategoryUpdateDto>(Item);
            await _service.UpdateAsync(Item.Id, dto);
            return NoContent();
        }
    }
}
