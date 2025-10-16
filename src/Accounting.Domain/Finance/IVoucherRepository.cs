using System;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance
{
    public interface IVoucherRepository: IRepository<Voucher, Guid>
    {
    }
}
