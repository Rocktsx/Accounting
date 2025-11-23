using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.PayableVouchers
{
    public interface IPayableVoucherAppService: IVoucherAppService
    {
        /// <summary>
        /// 通过客户id获取收款明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<PayableDetailDto>> GetPayableDetailsByDebitorAsync
            (PayableDetailByDebitorRequestDto input);
        /// <summary>
        /// 通过传票id获取收款明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<PayableDetailDto>> GetPayableDetailsAsync(Guid id);
       
    }
}
