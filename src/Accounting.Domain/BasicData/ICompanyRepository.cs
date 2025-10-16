using System;
using Volo.Abp.Domain.Repositories;

namespace Accounting.BasicData
{
    public interface ICompanyRepository : IRepository<Company, Guid>
    {
    }
}
