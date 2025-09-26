using Accounting.Localization;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;

namespace Accounting;

/* Inherit your application services from this class.
 */
public abstract class AccountingAppService : ApplicationService
{
    protected AccountingAppService()
    {
        LocalizationResource = typeof(AccountingResource);
    }
    protected async Task<bool> CheckPermissions(params string[] permissons)
    {
        if (!await AuthorizationService.IsGrantedAnyAsync(permissons))
        {
            throw new UnauthorizedAccessException();
        }
        return true;
    }
}
