using Accounting.Common;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.ProfitAndLossReports;
using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.ObjectMapping;

namespace Accounting.Finance.GeneralLedgerReports
{
    public class ProfitAndLossReportAppService : AccountingAppService, IProfitAndLossReportAppService
    {
        private readonly IProfitAndLossReportRepository _plRepository;
        private readonly IAccountingPeriodRepository _periodRepository;

        public ProfitAndLossReportAppService(IProfitAndLossReportRepository repository,
            IAccountingPeriodRepository periodRepository)
        {
            _plRepository = repository;
            _periodRepository = periodRepository;
        }
        [Authorize(AccountingPermissions.ProfitAndLossReports.MonthToDateYearToDateReport)]
        public async Task<IEnumerable<ProfitAndLossMonthToDateYearToDateResultDto>> GetMtdYtdListAsync(ProfitAndLossMtdYtdRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);
            var startDate = period.GetStartDate(input.StartDate);

            var list = await _plRepository.GetMonthToDateAndYearToDateListAsync(startDate, endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {   
                var dto = ObjectMapper.Map<ProfitAndLossMonthToDateYearToDateResult, ProfitAndLossMonthToDateYearToDateResultDto>(item);

                SetGroupProperty(dto, item.AccountTypeId, accountTppeGroups);

                return dto;
            }).ToList();

            return result;
        }

        [Authorize(AccountingPermissions.ProfitAndLossReports.YearToDateReport)]
        public async Task<IEnumerable<ProfitAndLossYearToDateResultDto>> GetYtdListAsync(ProfitAndLossYearToDateRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);

            var list = await _plRepository.GetYearToDateListAsync(endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {  
                var dto = ObjectMapper.Map<ProfitAndLossYearToDateResult, ProfitAndLossYearToDateResultDto>(item);

                SetGroupProperty(dto, item.AccountTypeId, accountTppeGroups);

                return dto;
            }).ToList();

            return result;
        }
        private async Task<(AccountingPeriod period, DateOnly endDate)> HandleRequestDto(ProfitAndLossYearToDateRequestDto input)
        {
            Check.NotDefaultOrNull(input.PeriodId, nameof(input.PeriodId));

            var period = await _periodRepository.FindAsync(input.PeriodId.Value) ?? throw new BusinessException(VoucherErrorCodes.AccountingPeriodNotFound);

            var endDate = period.GetEndDate(input.EndDate);
            return (period, endDate);
        }

        [Authorize(AccountingPermissions.ProfitAndLossReports.TwelveMonthsReport)]
        public async Task<IEnumerable<ProfitAndLoss12MonthsResultDto>> GetTwelveMonthsAsync(
            ProfitAndLossMtdYtdRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);
            var startDate = period.GetStartDate(input.StartDate);

            var list = await _plRepository.Get12MonthsListAsync(startDate, endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {  
                var dto = ObjectMapper.Map< ProfitAndLoss12MonthsResult, ProfitAndLoss12MonthsResultDto>(item);

                SetGroupProperty(dto, item.AccountTypeId, accountTppeGroups);

                return dto;
            }).ToList();

            return result;
        }
    }
}
