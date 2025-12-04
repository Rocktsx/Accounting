using Accounting.Common;
using Accounting.Finance.PayableVouchers;
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
    public class PayableVoucherAppService : VoucherAppService, IPayableVoucherAppService
    {
        public PayableVoucherAppService(IVoucherRepository repository) : base(repository)
        {
            FunctionCode = FunctionCodes.PayableVoucher;

            GetPolicyName = AccountingPermissions.PayableVouchers.Default;
            GetListPolicyName = AccountingPermissions.PayableVouchers.Default;
            CreatePolicyName = AccountingPermissions.PayableVouchers.Create;
            UpdatePolicyName = AccountingPermissions.PayableVouchers.Update;
            DeletePolicyName = AccountingPermissions.PayableVouchers.Delete;
            UpdateStatuePolicyName = AccountingPermissions.PayableVouchers.UpdateStatus;
        }
       
        public override Task<VoucherDto> CreateAsync(VoucherCreateDto input)
        {
            input.VoucherType = VoucherType.PayableVoucher;
            return base.CreateAsync(input);
        }

        public override Task<PagedResultDto<VoucherDto>> GetListAsync(VoucherFilterRequestDto input)
        {
            input.VoucherType = VoucherType.PayableVoucher;
            return base.GetListAsync(input);
        }
     
        [Authorize(AccountingPermissions.PayableVouchers.Default)]
        public async Task<IEnumerable<VoucherDetailDto>> GenerateDetailsAsync(GeneratePayableDetailRequestDto input)
        {
            using var generator = new PayableVoucherDetailGenerator(
                LazyServiceProvider, input);
            return await generator.GenerateAsync();
        }
        [Authorize(AccountingPermissions.PayableVouchers.Default)]
        public async Task<IEnumerable<PayableDetailDto>> GetPayableDetailsAsync(Guid id)
        {
            if (id.IsEmpty())
            {
                return [];
            }

            var list = await Repository.GetPayableDetailsAsync(id);
            return ObjectMapper.Map<IEnumerable<ReceivablePayableDetail>, List<PayableDetailDto>>([.. list]);
        }
        [Authorize(AccountingPermissions.PayableVouchers.Default)]
        public async Task<PagedResultDto<PayableDetailDto>> GetPayableDetailsByDebitorAsync(
            PayableDetailByDebitorRequestDto input)
        {
            if (input.DebitorId.IsEmptyOrNull())
            {
                return new PagedResultDto<PayableDetailDto>();
            }

            var count = await Repository.GetPayableDetailsCountAsync(input.DebitorId);

            var list = await Repository.GetPayableDetailsAsync(input.DebitorId, input.MaxResultCount, input.SkipCount);
            var dtos = ObjectMapper.Map<IEnumerable<ReceivablePayableDetail>, List<PayableDetailDto>>([.. list]);

            return new PagedResultDto<PayableDetailDto>(count, dtos);
        } 
    }
}
