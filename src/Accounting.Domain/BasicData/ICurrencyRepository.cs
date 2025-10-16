using System;
using Volo.Abp.Domain.Repositories;

namespace Accounting.BasicData
{
    public interface ICurrencyRepository: IRepository<Currency, Guid>
    {
    }
}
