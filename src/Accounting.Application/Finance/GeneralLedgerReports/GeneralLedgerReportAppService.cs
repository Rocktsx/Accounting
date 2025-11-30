using Accounting.Common;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Finance.GeneralLedgerReports
{
    public class GeneralLedgerReportAppService : AccountingAppService, IGeneralLedgerReportAppService

    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IAccountingPeriodRepository _periodRepository;
        public GeneralLedgerReportAppService(IVoucherRepository voucherRepository,
            IAccountingPeriodRepository accountingPeriodRepository)
        {
            _voucherRepository = voucherRepository;
            _periodRepository = accountingPeriodRepository;
        }
        public async Task<GeneralLedgerSingleCurrencyReportResultDto> GetSingleCurrencyList(
            GeneralLedgerSingleCurrencyReportRequestDto input)
        {
            Check.NotDefaultOrNull(input.PeriodId, nameof(input.PeriodId));

            var period = await _periodRepository.FindAsync(input.PeriodId.Value) ?? throw new BusinessException("AccountingPeriodNotFound", "The specified accounting period was not found.");

            var query =await _voucherRepository.WithDetailsAsync(item => item.Details);
            query = query.Where(new NoVoidVoucherSpecification().ToExpression());
            query = query.WhereIf(!input.SubjectId.IsEmptyOrNull(),
                item => item.Details.Any(d => d.SubjectId == input.SubjectId));

            var beforePeriodQuery = query.Where(item => item.VoucherDate < period.StartDate);

            throw new NotImplementedException();
        }
    }
}
