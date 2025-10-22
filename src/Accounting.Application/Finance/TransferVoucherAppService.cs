using Accounting.Common;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Finance
{
    [RemoteService(true)]
    public class TransferVoucherAppService : VoucherAppService, ITransferVoucherAppService
    {
        public TransferVoucherAppService(IVoucherRepository repository) : base(repository)
        {
            DeletePolicyName = AccountingPermissions.TransferVouchers.Delete;
            GetListPolicyName = AccountingPermissions.TransferVouchers.Default;
            GetPolicyName = AccountingPermissions.TransferVouchers.Default;

            FunctionCode = FunctionCodes.JournalVoucher;
        }

        [Authorize(AccountingPermissions.TransferVouchers.Create)]
        public override Task<VoucherDto> CreateAsync(VoucherCreateDto input)
        {
            input.VoucherType = VoucherType.JournalVoucher;
            foreach (var item in input.Details)
            {
                item.IsOriginal = true;
            }
            return base.CreateAsync(input);
        }

        [Authorize(AccountingPermissions.TransferVouchers.Update)]
        public override Task<VoucherDto> UpdateAsync(Guid id, VoucherUpdateDto input)
        {
            foreach (var item in input.Details)
            {
                item.IsOriginal = true;
            }
            return base.UpdateAsync(id, input);
        }

        protected override async Task<IQueryable<Voucher>> CreateFilteredQueryAsync(VoucherFilterRequestDto input)
        {
            input.VoucherType = VoucherType.JournalVoucher;
            return await base.CreateFilteredQueryAsync(input);
        }

        [Authorize(AccountingPermissions.TransferVouchers.UpdateStatus)]
        public override Task UpdateStatus(Guid id, VoucherStatus status)
        {
            return base.UpdateStatus(id, status);
        }
    }
}
