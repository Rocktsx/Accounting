using Accounting.Common;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using Accounting.Localization;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Accounting;

/* Inherit your application services from this class.
 */
public abstract class AccountingAppService : ApplicationService
{
    public const int DefaultAgingDays = 7;
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
    protected async Task<Dictionary<Guid, AccountTypeRoot>> GetAccountTypeGroupsAsync()
    {
        var accountTypeRepository = LazyServiceProvider.LazyGetRequiredService<IAccountTypeRepository>();
        var accountTypes = await accountTypeRepository.GetListAsync();

        var result = new Dictionary<Guid, AccountTypeRoot>();
        var dics = accountTypes.ToDictionary(item => item.Id, item => item);
        foreach (var item in accountTypes)
        {
            var rootGroup = item;
            var secondaryGroup = item;
            while (rootGroup.ParentId != null && dics.ContainsKey(rootGroup.ParentId.Value))
            {
                rootGroup = dics[rootGroup.ParentId.Value];

                if (rootGroup.ParentId != null)
                {
                    secondaryGroup = rootGroup;
                }
            }

            result[item.Id] = new AccountTypeRoot
            {
                Item = item,
                RootItem = rootGroup,
                SecondaryRootItem = secondaryGroup
            };
        }
        return result;
    }
    protected static void SetGroupProperty(ReportGroupBaseResultDto dto, Guid? accountTypeId, Dictionary<Guid, AccountTypeRoot> rootGroups)
    {
        if (accountTypeId != null && rootGroups.TryGetValue(accountTypeId.Value, out var group))
        {
            dto.GroupCode = group.RootItem?.Code ?? string.Empty;
            dto.GroupName = group.RootItem?.Name ?? string.Empty;
            dto.GroupOtherName = group.RootItem?.OtherName ?? string.Empty;
            dto.SecondaryGroupCode = group.SecondaryRootItem?.Code ?? string.Empty;
            dto.SecondaryGroupName = group.SecondaryRootItem?.Name ?? string.Empty;
            dto.SecondaryGroupOtherName = group.SecondaryRootItem?.OtherName ?? string.Empty;
        }
    }
    protected static void CheckPeriodId(Guid? periodId)
    {
        if (periodId.IsEmptyOrNull())
        {
            throw new BusinessException(VoucherErrorCodes.AccountingPeriodIdCanNotNull);
        }
    }
}
