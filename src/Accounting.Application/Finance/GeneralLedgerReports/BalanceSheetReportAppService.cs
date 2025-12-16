using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.BalanceSheetReports;
using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Finance.GeneralLedgerReports
{
    public class BalanceSheetReportAppService : AccountingAppService, IBalanceSheetReportAppService
    {
        private readonly IBalanceSheetReportRepository _bsRepository; 
        private readonly IAccountingPeriodRepository _periodRepository;

        public BalanceSheetReportAppService(IBalanceSheetReportRepository bsRepository, IAccountingPeriodRepository periodRepository)
        {
            _bsRepository = bsRepository;
            _periodRepository = periodRepository;
        }

        public async Task<IEnumerable<BalanceSheetMonthToDateYearToDateResultDto>> GetMtdYtdListAsync(BalanceSheetMtdYtdRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);
            var startDate = period.GetStartDate(input.StartDate);

            var list = await _bsRepository.GetMonthToDateAndYearToDateListAsync(startDate, endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {
                var dto = ObjectMapper.Map<BalanceSheetMonthToDateYearToDateResult, BalanceSheetMonthToDateYearToDateResultDto>(item);

                SetGroupProperty(dto, item.AccountTypeId, accountTppeGroups);

                return dto;
            }).ToList();

            return result;
        }

        public async Task<IEnumerable<BalanceSheetYearToDateResultDto>> GetYtdListAsync(BalanceSheetYearToDateRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);

            var list = await _bsRepository.GetYearToDateListAsync(endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {
                var dto = ObjectMapper.Map<BalanceSheetYearToDateResult, BalanceSheetYearToDateResultDto>(item);

                SetGroupProperty(dto, item.AccountTypeId, accountTppeGroups);

                return dto;
            }).ToList();

            return result;
        }
        private async Task<(AccountingPeriod period, DateOnly endDate)> HandleRequestDto(BalanceSheetYearToDateRequestDto input)
        {
            Check.NotDefaultOrNull(input.PeriodId, nameof(input.PeriodId));

            var period = await _periodRepository.FindAsync(input.PeriodId.Value) ?? throw new BusinessException(VoucherErrorCodes.AccountingPeriodNotFound);

            var endDate = period.GetEndDate(input.EndDate);
            return (period, endDate);
        }
    }
}
