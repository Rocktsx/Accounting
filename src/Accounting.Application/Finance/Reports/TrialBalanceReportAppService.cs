using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.TrialBalanceReports;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Finance.Reports
{
    public class TrialBalanceReportAppService : AccountingAppService, ITrialBalanceReportAppService
    {
        private readonly ITrialBalanceReportRepository _tbRepository;
        private readonly IAccountingPeriodRepository _periodRepository;

        public TrialBalanceReportAppService(ITrialBalanceReportRepository tbRepository,
            IAccountingPeriodRepository periodRepository)
        {
            _tbRepository = tbRepository;
            _periodRepository = periodRepository;
        }

        [Authorize(AccountingPermissions.TrialBalanceReports.MonthToDateYearToDateReport)]
        public async Task<IEnumerable<TrialBalanceMonthToDateYearToDateResultDto>> GetMtdYtdListAsync(TrialBalanceMtdYtdRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);
            var startDate = period.GetStartDate(input.StartDate);

            var list = await _tbRepository.GetMonthToDateAndYearToDateListAsync(startDate, endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {
                var dto = ObjectMapper.Map<TrialBalanceMonthToDateYearToDateResult, TrialBalanceMonthToDateYearToDateResultDto>(item);

                SetGroupProperty(dto, item.AccountTypeId, accountTppeGroups);

                return dto;
            }).ToList();

            return result;
        }

        [Authorize(AccountingPermissions.TrialBalanceReports.YearToDateReport)]
        public async Task<IEnumerable<TrialBalanceYearToDateResultDto>> GetYtdListAsync(TrialBalanceYtdRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);

            var list = await _tbRepository.GetYearToDateListAsync(endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {
                var dto = ObjectMapper.Map<TrialBalanceYearToDateResult, TrialBalanceYearToDateResultDto>(item);

                SetGroupProperty(dto, item.AccountTypeId, accountTppeGroups);

                return dto;
            }).ToList();

            return result;
        }
        private async Task<(AccountingPeriod period, DateOnly endDate)> HandleRequestDto(TrialBalanceYtdRequestDto input)
        {
            CheckPeriodId(input.PeriodId);

            var period = await _periodRepository.FindAsync(input.PeriodId.Value) ?? throw new BusinessException(VoucherErrorCodes.AccountingPeriodNotFound);

            var endDate = period.GetEndDate(input.EndDate);
            return (period, endDate);
        }
    }
}
