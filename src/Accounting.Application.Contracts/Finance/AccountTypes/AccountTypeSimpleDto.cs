using Accounting.Dtos;
using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.AccountTypes
{
    public class AccountTypeSimpleDto : SimpleDto<Guid>
    { 
        public AccountTypeTypes Category {  get; set; }
    }
}
