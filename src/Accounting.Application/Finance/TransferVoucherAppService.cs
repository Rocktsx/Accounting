using Accounting.Common;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

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
            CreatePolicyName = AccountingPermissions.TransferVouchers.Create;
            UpdatePolicyName = AccountingPermissions.TransferVouchers.Update;
            UpdateStatuePolicyName = AccountingPermissions.TransferVouchers.UpdateStatus;

            FunctionCode = FunctionCodes.JournalVoucher;
        }

        public override Task<VoucherDto> CreateAsync(VoucherCreateDto input)
        {
            input.VoucherType = VoucherType.JournalVoucher;
          
            return base.CreateAsync(input);
        }

        public override Task<PagedResultDto<VoucherDto>> GetListAsync(VoucherFilterRequestDto input)
        {
            input.VoucherType = VoucherType.JournalVoucher;

            return base.GetListAsync(input);
        }
    }
}
