using Accounting.Finance;
using Accounting.Finance.Dtos;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Accounting.Web.Pages.GenenalLedger.SubjectCategory
{
    public class CreateModalModel : AccountingPageModel
    { 
        private readonly ISubjectCategoryAppService _service;
        [BindProperty]
        public CreateSubjectCategoryViewModel Item { get; set; }
        public List<SelectListItem> AccountTypes { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public CreateModalModel(ISubjectCategoryAppService service)
        {
            _service = service;
        }
        public async Task OnGet()
        {
            Item = new CreateSubjectCategoryViewModel();
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
        public async Task<IActionResult> OnPost()
        {
            var dto = ObjectMapper.Map<CreateSubjectCategoryViewModel, SubjectCategoryCreateDto>(Item); 
            await _service.CreateAsync(dto);
            return NoContent();
        } 
    }
}
