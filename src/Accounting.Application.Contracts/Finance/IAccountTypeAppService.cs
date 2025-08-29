using Accounting.Finance.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Accounting.Finance
{
    public interface IAccountTypeAppService: IApplicationService, ICrudAppService<AccountTypeDto,Guid, FilteredPagedAndSortedResultRequestDto, AccountTypeCreateDto, AccountTypeUpdateDto>
    { 
        Task<IEnumerable<AccountTypeSelectDto>> GetSelectListAsync();
    }
}
