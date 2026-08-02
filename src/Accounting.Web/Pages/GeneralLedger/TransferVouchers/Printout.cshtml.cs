using Accounting.Common;
using Accounting.Finance.Settings;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.GeneralLedger.TransferVouchers
{
    [Authorize(Permissions.AccountingPermissions.TransferVouchers.Print)]
    public class PrintoutModel : PageModel
    {
        private readonly ITransferVoucherAppService _transferVoucherAppService;
        private readonly IAccountingSettingAppService _accountingSettingAppService;
        private readonly ISubjectAppService _subjectAppService;

        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        public VoucherDto? Voucher { get; set; }
        public AccountingSettingCompanyDto? CompanySetting { get; set; }

        public Dictionary<Guid, SubjectFilterResultDto>? Subjects { get; set; } = [];
        public PrintoutModel(ITransferVoucherAppService transferVoucherAppService,
            IAccountingSettingAppService accountingSettingAppService,
            ISubjectAppService subjectAppService)
        {
            _transferVoucherAppService = transferVoucherAppService;
            _accountingSettingAppService = accountingSettingAppService;
            _subjectAppService = subjectAppService;
        }
        public async Task OnGet()
        {
            if (!Id.IsEmptyOrNull())
            {
                Voucher = await _transferVoucherAppService.GetAsync(Id.Value);
            }
            CompanySetting = await _accountingSettingAppService.GetCompanyAsync();

            if (Voucher != null)
            {
                var subjects = await _subjectAppService.GetListAsync(new SubjectFilterRequestDto
                {
                    SubjectIds = Voucher.Details.Select(item => item.SubjectId).ToArray()
                });

                Subjects = subjects.Items.ToDictionary(item => item.Id, item => item);
            }
        }
    }
}
