using Accounting.EntityFrameworkCore;
using Accounting.Finance.Reports;
using Accounting.Finance.Vouchers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.Finance
{
    public class JournalReportRepository : VoucherRepository, IJournalReportRepository
    {
        public JournalReportRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public async Task<IEnumerable<JournalReportMultipleCurrencyResult>> GetJLMultipleCurrencyListAsync(
             JournalReportRequest request, string? sorting = null, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync(request, sorting);
            return await queryable.SelectMany(item => item.Details).Select(item => new JournalReportMultipleCurrencyResult
            {
                VoucherCode = item.Voucher.Code,
                VoucherDate = item.Voucher.VoucherDate,
                SubjectCode = item.Subject.Code,
                SubjectName = item.Subject.Name,
                SubjectOtherName = item.Subject.OtherName,
                CurrencyCode = item.CurrencyCode,
                ForeignAmount = item.ForeignAmount,
                CurrencyRate = item.CurrencyRate,
                DebitorCreditor = item.DebitorCreditor,
                NativeAmount = item.NativeAmount,
                Description = item.Description
            }).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<JournalReportSingleCurrencyResult>> GetJLSingleCurrencyListAsync(
            JournalReportRequest request, string? sorting = null, CancellationToken cancellationToken = default)
        {
            var queryable = await GetQueryableAsync(request, sorting);
            return await queryable.SelectMany(item => item.Details).Select(item => new JournalReportSingleCurrencyResult
            {
                VoucherCode = item.Voucher.Code,
                VoucherDate = item.Voucher.VoucherDate,
                SubjectCode = item.Subject.Code,
                SubjectName = item.Subject.Name,
                SubjectOtherName = item.Subject.OtherName,
                NativeAmount = item.NativeAmount,
                DebitorCreditor = item.DebitorCreditor,
                Description = item.Description
            }).ToListAsync(cancellationToken);
        }

        private async Task<IQueryable<Voucher>> GetQueryableAsync(JournalReportRequest request, string? sorting = null)
        {
            var voucherQueryable = await GetQueryableWithDetailsAsync();
            var queryable = voucherQueryable.WhereIf(request.StartDate != null,
                item => item.VoucherDate >= request.StartDate)
                .WhereIf(request.EndDate != null, item => item.VoucherDate <= request.EndDate)
                .WhereIf(!string.IsNullOrWhiteSpace(request.Prefix), item => item.Prefix.Contains(request.Prefix))
                .WhereIf(request.StartNo != null, item => item.GenNo >= request.StartNo)
                .WhereIf(request.EndNo != null, item => item.GenNo <= request.EndNo)
                .WhereIf(request.VoucherType != null, item => item.VoucherType == request.VoucherType)
                .OrderBy(!string.IsNullOrWhiteSpace(sorting) ? sorting : nameof(Voucher.Code));

            return queryable;
        }
    }
}
