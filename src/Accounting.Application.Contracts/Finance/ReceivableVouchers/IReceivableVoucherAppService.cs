using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.ReceivableVouchers
{
    public interface IReceivableVoucherAppService : IVoucherAppService
    {
        /// <summary>
        /// 通过客户id获取收款明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ReceivableDetailDto>> GetReceivableDetailsByCreditorAsync
            (ReceivableDetailsByCreditorRequestDto input);
        /// <summary>
        /// 通过传票id获取收款明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<ReceivableDetailDto>> GetReceivableDetailsAsync(Guid id);
        /// <summary>
        /// 生成传票明细
        /// </summary>
        /// <param name="input">input</param>
        /// <returns></returns>
        Task<IEnumerable<VoucherDetailDto>> GenerateDetailsAsync(
            GenerateReceivableDetailRequestDto input);
    }
}
