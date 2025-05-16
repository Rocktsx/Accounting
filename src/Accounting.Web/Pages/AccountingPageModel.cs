using Accounting.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Accounting.Web.Pages;

public abstract class AccountingPageModel : AbpPageModel
{
    protected AccountingPageModel()
    {
        LocalizationResourceType = typeof(AccountingResource);
    }
}
