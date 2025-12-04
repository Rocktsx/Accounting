using Accounting.Common;
using Accounting.Finance.ReceivableVouchers;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance
{
    [RemoteService(true)]
    public class ReceivableVoucherAppService : VoucherAppService, IReceivableVoucherAppService
    {
        public ReceivableVoucherAppService(IVoucherRepository repository) : base(repository)
        {
            GetPolicyName = AccountingPermissions.PayableVouchers.Default;
            GetListPolicyName = AccountingPermissions.PayableVouchers.Default;
            CreatePolicyName = AccountingPermissions.PayableVouchers.Create;
            UpdatePolicyName = AccountingPermissions.PayableVouchers.Update;
            DeletePolicyName = AccountingPermissions.PayableVouchers.Delete;
            UpdateStatuePolicyName = AccountingPermissions.PayableVouchers.UpdateStatus;

            FunctionCode = FunctionCodes.ReceivableVoucher;
        }

        public override Task<VoucherDto> CreateAsync(VoucherCreateDto input)
        {
            input.VoucherType = VoucherType.ReceivableVoucher;
            return base.CreateAsync(input);
        }

        public override Task<PagedResultDto<VoucherDto>> GetListAsync(VoucherFilterRequestDto input)
        {
            input.VoucherType = VoucherType.ReceivableVoucher; 
            return base.GetListAsync(input);
        }  
      
        /// <summary>
        /// 通过客户id获取收款明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(AccountingPermissions.ReceivableVouchers.Default)]
        public async Task<PagedResultDto<ReceivableDetailDto>>
            GetReceivableDetailsByCreditorAsync(ReceivableDetailsByCreditorRequestDto input)
        {
            if (input.CreditorId.IsEmptyOrNull())
            {
                return new PagedResultDto<ReceivableDetailDto>();
            }
             
            var count = await Repository.GetReceivableDetailsCountAsync(input.CreditorId);
         
            var list = await Repository.GetReceivableDetailsAsync(input.CreditorId, input.MaxResultCount, input.SkipCount);
            var dtos = ObjectMapper.Map<List<ReceivablePayableDetail>, List<ReceivableDetailDto>>([.. list]);
            return new PagedResultDto<ReceivableDetailDto>(count, dtos);
        }
        /// <summary>
        /// 通过传票id获取收款明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(AccountingPermissions.ReceivableVouchers.Default)]
        public async Task<IEnumerable<ReceivableDetailDto>>
            GetReceivableDetailsAsync(Guid id)
        {
            if (id.IsEmpty())
            {
                return [];
            }
            var list = await Repository.GetReceivableDetailsAsync(id);
            return ObjectMapper.Map<List<ReceivablePayableDetail>, List<ReceivableDetailDto>>([.. list]);
        } 
        /// <summary>
        /// 生成传票明细
        /// </summary>
        /// <param name="input">input</param>
        /// <returns></returns>
        [Authorize(AccountingPermissions.ReceivableVouchers.Default)]
        public async Task<IEnumerable<VoucherDetailDto>> GenerateDetailsAsync(
            GenerateReceivableDetailRequestDto input)
        {
            using var generator = new ReceivableVoucherDetailGenerator(
                LazyServiceProvider, input);
            return await generator.GenerateAsync();
        }
    }
}
