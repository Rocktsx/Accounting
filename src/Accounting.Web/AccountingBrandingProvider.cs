using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Accounting.Localization;

namespace Accounting.Web;

[Dependency(ReplaceServices = true)]
public class AccountingBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<AccountingResource> _localizer;

    public AccountingBrandingProvider(IStringLocalizer<AccountingResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
