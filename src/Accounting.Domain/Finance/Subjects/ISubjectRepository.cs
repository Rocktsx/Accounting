using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.Subjects
{
    public interface ISubjectRepository: IRepository<Subject, Guid>
    {
    }
}
