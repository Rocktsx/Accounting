using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.JournalReports;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Finance.GeneralLedgerReports
{
    /// <summary>
    /// 日志帐报表应用服务
    /// </summary>
    public class JournalReportAppService : AccountingAppService, IJournalReportAppService
    {
        private readonly IVoucherRepository _repository;

        public JournalReportAppService(IVoucherRepository repository)
        {
            _repository = repository;
        }

        [Authorize(AccountingPermissions.JournalReports.MultipleCurrencySortByDateReport)]
        public async Task<IEnumerable<VoucherDto>> GetMultipleCurrencySortByDateListAsync(JournalReportRequestDto input)
        {
            return await GetListAsync(input, nameof(Voucher.VoucherDate));
        }

        [Authorize(AccountingPermissions.JournalReports.MultipleCurrencySortByCodeReport)]
        public async Task<IEnumerable<VoucherDto>> GetMultipleCurrencySortByCodeListAsync(JournalReportRequestDto input)
        {
            return await GetListAsync(input, nameof(Voucher.Code));
        }

        [Authorize(AccountingPermissions.JournalReports.SingleCurrencySortByDateReport)]
        public async Task<IEnumerable<VoucherDto>> GetSingleCurrencySortByDateListAsync(JournalReportRequestDto input)
        {
            return await GetListAsync(input, nameof(Voucher.VoucherDate));
        }

        [Authorize(AccountingPermissions.JournalReports.SingleCurrencySortByCodeReport)]
        public async Task<IEnumerable<VoucherDto>> GetSingleCurrencySortByCodeListAsync(JournalReportRequestDto input)
        {
            return await GetListAsync(input, nameof(Voucher.Code));
        }
        
        private async Task<IEnumerable<VoucherDto>> GetListAsync(JournalReportRequestDto input, string sorting)
        {
            var filter = await HandleRequestDto(input);
            var list = await _repository.GetPagedListAsync(filter, sorting: sorting, includeDetails: true);

            return ObjectMapper.Map<IEnumerable<Voucher>, List<VoucherDto>>(list);
        }
        private async Task<VoucherFilterRequest> HandleRequestDto(JournalReportRequestDto input)
        {
            var filter = ObjectMapper.Map<JournalReportRequestDto, VoucherFilterRequest>(input);

            if (input.StartDate == null || input.EndDate == null)
            {
                var periodRepository = LazyServiceProvider.LazyGetRequiredService<IAccountingPeriodRepository>();
                var currentPeriods = await periodRepository.GetCurrentPeriodsAsync();
                if (input.StartDate == null)
                {
                    filter.StartDate = currentPeriods.Min(item => item.StartDate);
                }
                if (input.EndDate == null)
                {
                    filter.EndDate = currentPeriods.Max(item => item.EndDate);
                }
            }
            return filter;
        }
    }
}
