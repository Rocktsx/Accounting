using Accounting.Finance.ReceivableAgingReports;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    public class ReceivableAgingReportAppService : AccountingAppService, IReceivableAgingReportAppService
    {
        private readonly IReceivablePayableAgingReportRepository _agingReportRepository;

        public ReceivableAgingReportAppService(IReceivablePayableAgingReportRepository agingReportRepository)
        {
            _agingReportRepository = agingReportRepository;
        }

        [Authorize(AccountingPermissions.ReceivableAgingReports.AgingSummarySingleCurrency)]
        public async Task<IEnumerable<AgingSummarySingleCurrencyResultDto>> GetAgingSummarySingleCurrencyListAsync(AgingSummarySingleCurrencyRequestDto input)
        {
            var list = await _agingReportRepository.GetAgingSummarySingleCurrencyListAsync(input.SubSubjectCode,
                input.EndDate ?? DateOnly.FromDateTime(DateTime.Now), input.AgingDays ?? DefaultAgingDays);

            return ObjectMapper.Map<IEnumerable<AgingSummarySingleCurrencyResult>, IEnumerable<AgingSummarySingleCurrencyResultDto>>(list);
        }

        [Authorize(AccountingPermissions.ReceivableAgingReports.AgingSummaryMultipleCurrency)]
        public async Task<IEnumerable<AgingSummaryMultipleCurrencyResultDto>> GetAgingSummaryMultipleCurrencyListAsync(
           AgingSummarySingleCurrencyRequestDto input)
        {
            var list = await _agingReportRepository.GetAgingSummaryMultipleCurrencyListAsync(input.SubSubjectCode,
                input.EndDate ?? DateOnly.FromDateTime(DateTime.Now), input.AgingDays ?? DefaultAgingDays);

            return ObjectMapper.Map<IEnumerable<AgingSummaryMultipleCurrencyResult>, IEnumerable<AgingSummaryMultipleCurrencyResultDto>>(list);
        }
    }
}
