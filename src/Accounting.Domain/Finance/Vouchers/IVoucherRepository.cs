using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance.Vouchers
{
    public interface IVoucherRepository: IBasicRepository<Voucher, Guid>
    {
        Task<IEnumerable<Voucher>> GetPagedListAsync(
          VoucherFilterRequest request = null,
          string sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = 0,
          CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(
            VoucherFilterRequest request = null,
            Guid? subjectId = null,
            CancellationToken cancellationToken = default);

        Task<long> GetLastNumberAsync(string prefix, CancellationToken cancellationToken = default);

        Task <IEnumerable<ReceivablePayableDetail>> GetReceivableDetailsAsync(
            Guid? creditorId,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<ReceivablePayableDetail>> GetReceivableDetailsAsync(
           Guid id,
           CancellationToken cancellationToken = default);

        Task<long> GetReceivableDetailsCountAsync(
            Guid? creditorId = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<ReceivablePayableDetail>> GetPayableDetailsAsync(
            Guid? debitorId,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<ReceivablePayableDetail>> GetPayableDetailsAsync(
           Guid id,
           CancellationToken cancellationToken = default);

        Task<long> GetPayableDetailsCountAsync(
            Guid? debitorId = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DocNoRepeatResult>> GetDocNoRepeatsAsync( 
            IEnumerable<string>? docNos, 
            Guid voucherId,
            CancellationToken cancellationToken = default);
    }
}
