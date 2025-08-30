using Accounting.Finance;
using Accounting.Finance.Dtos;
using Accounting.Web.ViewModels;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GenenalLedger.AccountType
{
    public class CreateModalModel : AccountingPageModel
    {
        private IAccountTypeAppService _service;
        private readonly IServiceProvider _serviceProvider;
        [BindProperty]
        public CreateAccountTypeViewModel Item { get; set; }
        public List<SelectListItem> AccountTypes { get; set; }
        public CreateModalModel(IAccountTypeAppService service)
        {
            _service =service;
        }
        public async Task OnGet()
        {
            Item = new CreateAccountTypeViewModel();
            var dtos = await _service.GetSimpleListAsync();
            AccountTypes = dtos.Select(item =>new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = Helpers.GetText(item.Code, item.Name, item.OtherName),
            }).ToList(); 
            AccountTypes.Insert(0, new SelectListItem { Value = null, Text = "--" });
        }
        public async Task<IActionResult> OnPost()
        {  
            var dto = ObjectMapper.Map<CreateAccountTypeViewModel, AccountTypeCreateDto>(Item);
            await _service.CreateAsync(dto);
            return NoContent();
        }
    }
}
