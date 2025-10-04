using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounting.BasicData.Currencies;
using Accounting.Finance.Settings;
using Accounting.Finance.Subjects;
using Accounting.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc;

namespace Accounting.Web.Components.AccountingSettingGroup;

public class AccountingSettingGroupViewComponent : AbpViewComponent
{
    private readonly ICurrencyAppService _currencyAppService;
    private readonly ISubjectAppService _subjectAppService;
    private readonly IAccountingSettingAppService _accountingSettingAppService;
    public List<SelectListItem> Currencies { get; set; }
    public List<SelectListItem> Subjects { get; set; }
    public AccountingSettingViewModel Setting { get; set; }

    public AccountingSettingGroupViewComponent(ICurrencyAppService currencyAppService,
        ISubjectAppService subjectAppService, IAccountingSettingAppService accountingSettingAppService)
    {
        _currencyAppService = currencyAppService;
        _subjectAppService = subjectAppService;
        _accountingSettingAppService = accountingSettingAppService;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var currencyDtos = await _currencyAppService.GetActiveListAsync();
        Currencies = currencyDtos.ToSelectListItems(item => item.TargetCurrency, item => item.TargetCurrency);
        var subjectDtos = await _subjectAppService.GetSimpleListAsync();
        Subjects = subjectDtos.ToSelectListItems(item => item.Id.ToString(),
            item => Helpers.GetText(item.Code, item.Name, item.OtherName));
        var dto = await _accountingSettingAppService.GetAsync();
        Setting = ObjectMapper.Map<AccountingSettingDto, AccountingSettingViewModel>(dto);
        return View("~/Components/AccountingSettingGroup/Default.cshtml", this);
    }
}