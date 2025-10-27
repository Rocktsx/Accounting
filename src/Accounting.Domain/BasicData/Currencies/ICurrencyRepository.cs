using System;
using Volo.Abp.Domain.Repositories;

namespace Accounting.BasicData.Currencies
{
    public interface ICurrencyRepository: IRepository<Currency, Guid>
    {
    }
}
