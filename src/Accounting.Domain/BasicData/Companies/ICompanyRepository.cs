using System;
using Volo.Abp.Domain.Repositories;

namespace Accounting.BasicData.Companies
{
    public interface ICompanyRepository : IRepository<Company, Guid>
    {
    }
}
