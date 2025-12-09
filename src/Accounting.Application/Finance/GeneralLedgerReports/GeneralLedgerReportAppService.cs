using Accounting.Common;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
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
            GeneralLedgerSingleCurrencyReportRequestDto input)
        {
            Check.NotDefaultOrNull(input.PeriodId, nameof(input.PeriodId));

            var period = await _periodRepository.FindAsync(input.PeriodId.Value) ?? throw new BusinessException(VoucherErrorCodes.AccountingPeriodNotFound);
            var startDate = input.StartDate ?? period.StartDate;
            var endDate = input.EndDate ?? period.EndDate;

            var result =await _glRepository.GetGLSingleCurrencyListAsync(startDate, endDate, period.StartDate, period.EndDate, input.SubjectId);

            return ObjectMapper.Map<IEnumerable<GeneralLedgerSingleCurrencyReportResult>, List<GeneralLedgerSingleCurrencyReportResultDto>>([.. result]);
        }
    }
}
