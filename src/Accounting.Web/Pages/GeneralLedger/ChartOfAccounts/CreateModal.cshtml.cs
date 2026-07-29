using Accounting.BasicData.Currencies;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.ChartOfAccounts
{
    [Authorize(Permissions.AccountingPermissions.Subjects.Create)]
    public class CreateModalModel : AccountingPageModel
    {
        [BindProperty]
        public CreateSubjectViewModel Item { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid? SubjectCategoryId { get; set; }
        public List<SelectListItem> AccountTypes { get; set; } = [];
        public List<SelectListItem> Categories { get; set; } = [];
        public List<SelectListItem> Currencies { get; set; } = [];
        public CreateModalModel()
        {
            Item = new CreateSubjectViewModel();
        }
        public async Task OnGet()
        {
            Item = new CreateSubjectViewModel
            {
                SubjectCategoryId = SubjectCategoryId ?? null,
                IsActive = true
            };
            var accountTypeService = LazyServiceProvider.GetRequiredService<IAccountTypeAppService>();
            var dtos = await accountTypeService.GetSimpleListAsync();
            AccountTypes = dtos.ToSelectListItems(
                 item => item.Id.ToString(),
                 item => Helpers.GetText(item.Code, item.Name, item.OtherName));
            var categoryService = LazyServiceProvider.GetRequiredService<ISubjectCategoryAppService>();
            var categoryDtos = await categoryService.GetSimpleListAsync();
            Categories = categoryDtos.ToSelectListItems(
                item => item?.Id?.ToString() ?? string.Empty,
                item => Helpers.GetText(item.Code, item.Name, item.OtherName));
            var currencyAppService = LazyServiceProvider.GetRequiredService<ICurrencyAppService>();
            var currencies = await currencyAppService.GetActiveListAsync();
            Currencies = currencies.Select(c => new SelectListItem() { Text = c.TargetCurrency, Value = c.TargetCurrency }).ToList();
        }
        public async Task<IActionResult> OnPost()
        {
            var service = LazyServiceProvider.GetRequiredService<ISubjectAppService>();
            var dto = ObjectMapper.Map<CreateSubjectViewModel, SubjectCreateDto>(Item);
            await service.CreateAsync(dto);
            return NoContent();
        }
    }
}
