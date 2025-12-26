using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.BankReconciliations;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Finance.Reports
{
    public class BankReconciliationReportAppService : AccountingAppService, IBankReconciliationReportAppService
    {
        private readonly IBankReconciliationReportRepository _reportRepository;
        private readonly IAccountingPeriodRepository _periodRepository;

        public BankReconciliationReportAppService(IBankReconciliationReportRepository reportRepository,
            IAccountingPeriodRepository periodRepository)
        {
            _reportRepository = reportRepository;
            _periodRepository = periodRepository;
        }

        [Authorize(AccountingPermissions.BankReconciliationReports.Report)]
        public async Task<IEnumerable<BankReconciliationReportResultDto>> GetListAsync(
            BankReconciliationReportRequestDto input)
        {
            var (startDate, endDate) = await HandleRequestDtoAsync(input);
            var result = await _reportRepository.GetReportListAsync(
                    startDate, endDate, input.SubjectId);

            return ObjectMapper.Map<IEnumerable<BankReconciliationReportResult>,
                IEnumerable<BankReconciliationReportResultDto>>([.. result]);
        }

        [Authorize(AccountingPermissions.BankReconciliationReports.UnpresentedReport)]
        public async Task<IEnumerable<BankReconciliationUnpresentedReportResultDto>>
            GetUnpresentedListAsync(BankReconciliationReportRequestDto input)
        {
            var (startDate, endDate) = await HandleRequestDtoAsync(input);
            var result = await _reportRepository.GetUnpresentedReportListAsync(
                    startDate, endDate, input.SubjectId);

            return ObjectMapper.Map<IEnumerable<BankReconciliationUnpresentedReportResult>, 
                IEnumerable<BankReconciliationUnpresentedReportResultDto>>([.. result]);
        }
        private async Task<(DateOnly startDate, DateOnly endDate)> HandleRequestDtoAsync(
            BankReconciliationReportRequestDto input)
        {
            var currentPeriods = await _periodRepository.GetCurrentPeriodsAsync();

            var startDate = input.StartDate ?? currentPeriods.Min(item => item.StartDate);
            var endDate = input.EndDate ?? currentPeriods.Max(item => item.EndDate);

            return (startDate, endDate);
        }
    }
}
