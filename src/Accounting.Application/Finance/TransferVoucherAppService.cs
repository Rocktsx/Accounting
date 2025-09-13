using Accounting.Finance.Dtos;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Finance
{
    [RemoteService(true)]
    public class TransferVoucherAppService : VoucherAppService, ITransferVoucherAppService
    {
        public TransferVoucherAppService(IRepository<Voucher, Guid> repository) : base(repository)
        {
            DeletePolicyName = AccountingPermissions.TransferVoucherDeletion;
            GetListPolicyName = AccountingPermissions.TransferVoucher;
        }

        [Authorize(AccountingPermissions.TransferVoucherCreation)]
        public override Task<VoucherDto> CreateAsync(VoucherCreateDto input)
        {
            input.VoucherType = VoucherType.JournalVoucher;
            return base.CreateAsync(input);
        }

        [Authorize(AccountingPermissions.TransferVoucherEdit)]
        public override Task<VoucherDto> UpdateAsync(Guid id, VoucherUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }

        protected override async Task<IQueryable<Voucher>> CreateFilteredQueryAsync(VoucherFilterRequestDto input)
        {
            input.VoucherType = VoucherType.JournalVoucher;
            return await base.CreateFilteredQueryAsync(input);
        }
    }
}
