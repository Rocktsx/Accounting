using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Accounting.Finance.AccountTypes
{
    /// <summary>
    /// 科目类别
    /// </summary>
    public interface IAccountTypeAppService: IApplicationService, ICrudAppService<AccountTypeDto,Guid, FilteredPagedAndSortedResultRequestDto, AccountTypeCreateDto, AccountTypeUpdateDto>
    { 
        Task<IEnumerable<AccountTypeSimpleDto>> GetSimpleListAsync();
    }
}
