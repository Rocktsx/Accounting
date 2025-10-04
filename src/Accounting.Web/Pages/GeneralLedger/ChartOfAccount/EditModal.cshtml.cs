using Accounting.BasicData;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.ChartOfAccount
{
    public class EditModalModel : AccountingPageModel
    {
        private readonly ISubjectAppService _service;
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        [BindProperty]
        public EditSubjectViewModel Item { get; set; }
        public List<SelectListItem> AccountTypes { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public EditModalModel(ISubjectAppService service)
        {
            _service = service;
        }
        public async Task OnGet()
        {
            var dto = await _service.GetAsync(Id);
            Item = ObjectMapper.Map<SubjectDto, EditSubjectViewModel>(dto);
            var accountTypeService = LazyServiceProvider.GetRequiredService<IAccountTypeAppService>();
            var dtos = await accountTypeService.GetSimpleListAsync();
            AccountTypes = dtos.ToSelectListItems(
                 item => item.Id.ToString(),
                 item => Helpers.GetText(item.Code, item.Name, item.OtherName));
            var categoryService = LazyServiceProvider.GetRequiredService<ISubjectCategoryAppService>();
            var categoryDtos = await categoryService.GetSimpleListAsync();
            Categories = categoryDtos.ToSelectListItems(
                item => item.Id.ToString(),
                item => Helpers.GetText(item.Code, item.Name, item.OtherName));
            var currencyAppService = LazyServiceProvider.GetRequiredService<ICurrencyAppService>();
            var currencies = await currencyAppService.GetActiveListAsync();
            Currencies = currencies.Select(c => new SelectListItem() { Text = c.TargetCurrency, Value = c.TargetCurrency }).ToList();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var dto = ObjectMapper.Map<EditSubjectViewModel, SubjectUpdateDto>(Item);
            await _service.UpdateAsync(Item.Id, dto);
            return NoContent();
        }
    }
}
