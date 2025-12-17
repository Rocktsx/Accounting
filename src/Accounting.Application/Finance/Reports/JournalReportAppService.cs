using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.JournalReports; 
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    /// <summary>
    /// 日志帐报表应用服务
    /// </summary>
    public class JournalReportAppService : AccountingAppService, IJournalReportAppService
    {
        private readonly IJournalReportRepository _repository;

        public JournalReportAppService(IJournalReportRepository repository)
        {
            _repository = repository;
        }

        [Authorize(AccountingPermissions.JournalReports.MultipleCurrencySortByDateReport)]
        public async Task<IEnumerable<JournalMultipleCurrencyReportResultDto>> GetMultipleCurrencySortByDateListAsync(JournalReportRequestDto input)
        {
            return await GetMultipleCurrencyListAsync(input, nameof(Voucher.VoucherDate));
        }

        [Authorize(AccountingPermissions.JournalReports.MultipleCurrencySortByCodeReport)]
        public async Task<IEnumerable<JournalMultipleCurrencyReportResultDto>> GetMultipleCurrencySortByCodeListAsync(JournalReportRequestDto input)
        {
            return await GetMultipleCurrencyListAsync(input, nameof(Voucher.Code));
        }

        [Authorize(AccountingPermissions.JournalReports.SingleCurrencySortByDateReport)]
        public async Task<IEnumerable<JournalSingleCurrencyReportResultDto>> GetSingleCurrencySortByDateListAsync(JournalReportRequestDto input)
        {
            return await GetSingleCurrencyListAsync(input, nameof(Voucher.VoucherDate));
        }

        [Authorize(AccountingPermissions.JournalReports.SingleCurrencySortByCodeReport)]
        public async Task<IEnumerable<JournalSingleCurrencyReportResultDto>> GetSingleCurrencySortByCodeListAsync(JournalReportRequestDto input)
        {
            return await GetSingleCurrencyListAsync(input, nameof(Voucher.Code));
        }
        private async Task<IEnumerable<JournalMultipleCurrencyReportResultDto>> GetMultipleCurrencyListAsync(JournalReportRequestDto input, string sorting)
        {
            var filter = await HandleRequestDto(input);
            var list = await _repository.GetJLMultipleCurrencyListAsync(filter, sorting: sorting);

            return ObjectMapper.Map<IEnumerable<JournalReportMultipleCurrencyResult>, IEnumerable<JournalMultipleCurrencyReportResultDto>>(list);
        }
        private async Task<IEnumerable<JournalSingleCurrencyReportResultDto>> GetSingleCurrencyListAsync(JournalReportRequestDto input, string sorting)
        {
            var filter = await HandleRequestDto(input);
            var list = await _repository.GetJLSingleCurrencyListAsync(filter, sorting: sorting);

            return ObjectMapper.Map<IEnumerable<JournalReportSingleCurrencyResult>, IEnumerable<JournalSingleCurrencyReportResultDto>>(list);
        }
        private async Task<JournalReportRequest> HandleRequestDto(JournalReportRequestDto input)
        {
            var filter = ObjectMapper.Map<JournalReportRequestDto, JournalReportRequest>(input);

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
