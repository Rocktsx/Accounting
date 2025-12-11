using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Finance.GeneralLedgerReports
{
    /// <summary>
    /// 总账报表应用服务
    /// </summary>
    public class GeneralLedgerReportAppService : AccountingAppService, IGeneralLedgerReportAppService

    {
        private readonly IGeneralLedgerReportRepository _glRepository;
        private readonly IAccountingPeriodRepository _periodRepository;
        public GeneralLedgerReportAppService(IGeneralLedgerReportRepository repository,
            IAccountingPeriodRepository accountingPeriodRepository)
        {
            _glRepository = repository;
            _periodRepository = accountingPeriodRepository;
        }

        [Authorize(AccountingPermissions.GeneralLedgerReports.SingleCurrencyReport)]
        public async Task<IEnumerable<GeneralLedgerSingleCurrencyReportResultDto>> GetSingleCurrencyListAsync(
            GeneralLedgerReportRequestDto input)
        {
            (AccountingPeriod period, DateOnly startDate, DateOnly endDate) = await HandleRequestDto(input);

            var result = await _glRepository.GetGLSingleCurrencyListAsync(startDate, endDate, period.StartDate, period.EndDate, input.SubjectId);

            return ObjectMapper.Map<IEnumerable<GeneralLedgerSingleCurrencyReportResult>, List<GeneralLedgerSingleCurrencyReportResultDto>>([.. result]);
        }

        [Authorize(AccountingPermissions.GeneralLedgerReports.MultipleCurrencyReport)]
        public async Task<IEnumerable<GeneralLedgerMultipleCurrencyReportResultDto>>
            GetMultipleCurrencyListAsync(GeneralLedgerReportRequestDto input)
        {
            (AccountingPeriod period, DateOnly startDate, DateOnly endDate) = await HandleRequestDto(input);

            var result = await _glRepository.GetGLMultipleCurrencyListAsync(startDate, endDate, period.StartDate, period.EndDate, input.SubjectId);

            return ObjectMapper.Map<IEnumerable<GeneralLedgerMultipleCurrencyReportResult>, List<GeneralLedgerMultipleCurrencyReportResultDto>>([.. result]);
        }

        private async Task<(AccountingPeriod period, DateOnly startDate, DateOnly endDate)> HandleRequestDto(GeneralLedgerReportRequestDto input)
        {
            Check.NotDefaultOrNull(input.PeriodId, nameof(input.PeriodId));

            var period = await _periodRepository.FindAsync(input.PeriodId.Value) ?? throw new BusinessException(VoucherErrorCodes.AccountingPeriodNotFound);
            var startDate = input.StartDate ?? period.StartDate;
            var endDate = input.EndDate ?? period.EndDate;
            return (period, startDate, endDate);
        }
    }
}
