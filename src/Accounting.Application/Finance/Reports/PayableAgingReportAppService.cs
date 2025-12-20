using Accounting.Finance.PayableAgingReports;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    public class PayableAgingReportAppService: AccountingAppService, IPayableAgingReportAppService
    {
        private readonly IReceivablePayableAgingReportRepository _agingReportRepository;

        public PayableAgingReportAppService(IReceivablePayableAgingReportRepository agingReportRepository)
        {
            _agingReportRepository = agingReportRepository;
        }

        [Authorize(AccountingPermissions.PayableAgingReports.AgingSummarySingleCurrency)]
        public async Task<IEnumerable<AgingSummarySingleCurrencyResultDto>> GetAgingSummarySingleCurrencyListAsync(AgingReportRequestDto input)
        {
            var list = await _agingReportRepository.GetAgingSummarySingleCurrencyListAsync(input.SubSubjectCode,
                input.EndDate ?? DateOnly.FromDateTime(DateTime.Now), input.AgingDays ?? DefaultAgingDays, AccountTypeTypes.Payable);

            return ObjectMapper.Map<IEnumerable<AgingSummarySingleCurrencyResult>, IEnumerable<AgingSummarySingleCurrencyResultDto>>(list);
        }

        [Authorize(AccountingPermissions.PayableAgingReports.AgingSummaryMultipleCurrency)]
        public async Task<IEnumerable<AgingSummaryMultipleCurrencyResultDto>> GetAgingSummaryMultipleCurrencyListAsync(
           AgingReportRequestDto input)
        {
            var list = await _agingReportRepository.GetAgingSummaryMultipleCurrencyListAsync(input.SubSubjectCode,
                input.EndDate ?? DateOnly.FromDateTime(DateTime.Now), input.AgingDays ?? DefaultAgingDays, AccountTypeTypes.Payable);

            return ObjectMapper.Map<IEnumerable<AgingSummaryMultipleCurrencyResult>, IEnumerable<AgingSummaryMultipleCurrencyResultDto>>(list);
        }

        [Authorize(AccountingPermissions.PayableAgingReports.AgingDetail)]
        public async Task<IEnumerable<AgingDetailResultDto>> GetAgingDetailListAsync(
           AgingReportRequestDto input)
        {
            var list = await _agingReportRepository.GetAgingDetailListAsync(input.SubSubjectCode,
                input.EndDate ?? DateOnly.FromDateTime(DateTime.Now), AccountTypeTypes.Payable);

            return ObjectMapper.Map<IEnumerable<AgingDetailResult>, IEnumerable<AgingDetailResultDto>>(list);
        }
    }
}
