using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Finance.BankReconciliations
{
    public interface IBankReconciliationReportAppService
    {
        Task<IEnumerable<BankReconciliationReportResultDto>> GetListAsync(
            BankReconciliationReportRequestDto input);

        Task<IEnumerable<BankReconciliationUnpresentedReportResultDto>> GetUnpresentedListAsync(
            BankReconciliationReportRequestDto input);
    }
}
