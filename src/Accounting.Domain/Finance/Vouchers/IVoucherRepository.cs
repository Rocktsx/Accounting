using System;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.Vouchers
{
    public interface IVoucherRepository: IRepository<Voucher, Guid>
    {
    }
}
