using Accounting.Common;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.Reports;
using Accounting.Finance.TrialBalanceReports;
using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Finance.GeneralLedgerReports
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
        public async Task<IEnumerable<TrialBalanceMonthToDateYearToDateResultDto>> GetMtdYtdListAsync(TrialBalanceMtdYtdRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);
            var startDate = period.GetStartDate(input.StartDate);

            var list = await _tbRepository.GetMonthToDateAndYearToDateListAsync(startDate, endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {
                accountTppeGroups.TryGetValue(item.AccountTypeId.Value, out AccountTypeRootGroup rootItem);

                var dto = new TrialBalanceMonthToDateYearToDateResultDto
                {
                    SortOrder = item.SortOrder,
                    Group = item.Group,
                    SubjectCode = item.SubjectCode,
                    SubjectName = item.SubjectName,
                    SubjectOtherName = item.SubjectOtherName,
                    NativeAmount = item.NativeAmount,
                    MonthToDateNativeAmount = item.MonthToDateNativeAmount,
                    LastPeriodNativeAmount = item.LastPeriodNativeAmount
                };
                SetReportGroupDto(dto, rootItem);

                return dto;
            }).ToList();

            return result;
        }

        public async Task<IEnumerable<TrialBalanceYearToDateResultDto>> GetYtdListAsync(TrialBalanceYtdRequestDto input)
        {
            var (period, endDate) = await HandleRequestDto(input);

            var list = await _tbRepository.GetYearToDateListAsync(endDate, period.StartDate);

            var accountTppeGroups = await GetAccountTypeGroupsAsync();

            var result = list.Select(item =>
            {
                accountTppeGroups.TryGetValue(item.AccountTypeId.Value, out AccountTypeRootGroup rootItem);

                var dto = new TrialBalanceYearToDateResultDto
                {
                    SortOrder = item.SortOrder,
                    Group = item.Group,
                    SubjectCode = item.SubjectCode,
                    SubjectName = item.SubjectName,
                    SubjectOtherName = item.SubjectOtherName,
                    NativeAmount = item.NativeAmount
                };
                SetReportGroupDto(dto, rootItem);

                return dto;
            }).ToList();

            return result;
        }
        private async Task<(AccountingPeriod period, DateOnly endDate)> HandleRequestDto(TrialBalanceYtdRequestDto input)
        {
            Check.NotDefaultOrNull(input.PeriodId, nameof(input.PeriodId));

            var period = await _periodRepository.FindAsync(input.PeriodId.Value) ?? throw new BusinessException(VoucherErrorCodes.AccountingPeriodNotFound);

            var endDate = period.GetEndDate(input.EndDate);
            return (period, endDate);
        }
    }
}
