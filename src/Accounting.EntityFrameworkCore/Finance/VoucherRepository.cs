using Accounting.EntityFrameworkCore;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Accounting.Common;
using Accounting.Finance.Reports;

namespace Accounting.Finance
{
    public class VoucherRepository : EfCoreRepository<AccountingDbContext, Voucher, Guid>, IVoucherRepository,
        IGeneralLedgerReportRepository, IJournalReportRepository
    {
        public VoucherRepository(IDbContextProvider<AccountingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(VoucherFilterRequest request = null, Guid? subjectId = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync(request))
                .WhereIf(!subjectId.IsEmptyOrNull(), item => item.Details.Any(obj => obj.SubjectId == subjectId))
                .LongCountAsync(cancellationToken);
        }

        public async Task<IEnumerable<Voucher>> GetPagedListAsync(VoucherFilterRequest request = null, string sorting = null,
            int maxResultCount = int.MaxValue, int skipCount = 0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync(request, includeDetails))
             .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(Voucher.Id) : sorting)
             .Skip(skipCount).Take(maxResultCount)
             .ToListAsync(cancellationToken);
        }
        public override async Task<Voucher> GetAsync(Guid id, bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            var queryable = (await GetDbSetAsync()).AsQueryable();
            if (includeDetails == true)
            {
                queryable = queryable.Include(item => item.Details);
            }
            var entity = await queryable.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            return entity ?? throw new EntityNotFoundException();
        }
        public async Task<long> GetLastNumberAsync(string prefix, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
               .Where(item => item.Prefix == prefix)
               .OrderByDescending(item => item.GenNo)
               .Select(item => item.GenNo)
               .FirstOrDefaultAsync(cancellationToken);
        }
        private async Task<IQueryable<Voucher>> GetQueryableAsync(VoucherFilterRequest request, bool includeDetails = false)
        {
            var queryable = (await GetDbSetAsync()).AsQueryable();
            if (includeDetails == true)
            {
                queryable = queryable.Include(item => item.Details);
            }
            if (request == null)
            {
                return queryable;
            }

            return queryable
                .WhereIf(!string.IsNullOrWhiteSpace(request.Filter), item => item.Code.Contains(request.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(request.Prefix), item => item.Prefix == request.Prefix)
                .WhereIf(request.Codes != null && request.Codes.Count() > 0, item => request.Codes.Contains(item.Code))
                .WhereIf(request.StartNo != null, item => item.GenNo >= request.StartNo)
                .WhereIf(request.EndNo != null, item => item.GenNo <= request.EndNo)
                .WhereIf(request.StartDate != null, item => item.VoucherDate >= request.StartDate)
                .WhereIf(request.EndDate != null, item => item.VoucherDate <= request.EndDate)
                .WhereIf(request.VoucherType != null, item => item.VoucherType == request.VoucherType)
                .WhereIf(request.Status == null, new NoVoidVoucherSpecification())
                .WhereIf(request.Status != null, item => item.Status == request.Status)
                .WhereIf(!string.IsNullOrWhiteSpace(request.DocNo), item => item.Details.Any(obj => obj.DocNo.Contains(request.DocNo)));
        }
        private async Task<IQueryable<VoucherDetail>> GetReceivablePayableQueryableAsync(Guid? subSubjectCodeId)
        {
            var queryable = (await GetDbSetAsync()).AsQueryable();
            queryable = queryable.Where(new NoVoidVoucherSpecification());
            return queryable.SelectMany(item => item.Details)
                .Where(item => item.SubSubjectCode == subSubjectCodeId);
        }

        public async Task<IEnumerable<ReceivablePayableDetail>> GetReceivableDetailsAsync(Guid? creditorId, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var queryable = await GetReceivablePayableQueryableAsync(creditorId);

            var notReceivedQueryable = queryable
               .GroupBy(item => new { item.DocNo })
               .Where(grp => grp.Sum(item =>
                       item.NativeAmount * (int)item.DebitorCreditor) > 0)
               .Select(grp => new
               {
                   grp.FirstOrDefault(item => item.IsOriginal == true).Voucher.VoucherDate,
                   grp.Key.DocNo,
                   PaidNativeAmount = grp.Sum(item => item.IsOriginal == false ?
                       item.NativeAmount * (-(int)item.DebitorCreditor) : 0),
               });

            var pageQuerable = notReceivedQueryable
                .OrderBy(item => item.VoucherDate)
                .ThenBy(item => item.DocNo)
               .Skip(skipCount)
               .Take(maxResultCount);

            var notReceiveList = await pageQuerable.ToListAsync(cancellationToken);

            var notReceivedDic = notReceiveList.ToDictionary(
                    item => item.DocNo, item => item.PaidNativeAmount);
            var docNos = notReceiveList.Select(item => item.DocNo);
            var list = await GetReceivableDetailsAsync(queryable, docNos, notReceivedDic, cancellationToken);

            return list;
        }
        private async Task<IEnumerable<ReceivablePayableDetail>> GetReceivableDetailsAsync(
            IQueryable<VoucherDetail> queryable, IEnumerable<string> docNos,
            Dictionary<string, decimal> receiveds, CancellationToken cancellationToken = default)
        {
            var detailsQueryable = queryable.
                                    Where(obj => docNos.Contains(obj.DocNo)
                                        && obj.IsOriginal == true)
                                    .Select(item => new { item, item.Voucher.VoucherDate });

            var details = await detailsQueryable.ToListAsync(cancellationToken);

            var subjectIds = details.Select(item => item.item.SubjectId).Distinct();
            var subjectDic = await GetSubjectsAsync(subjectIds, cancellationToken);

            var roundScale = AccountingCommonConsts.AmountRoundScale;
            var list = details.Select(obj =>
            {
                var item = obj.item;
                decimal paidNativeAmount = 0m;
                receiveds.TryGetValue(item.DocNo, out paidNativeAmount);
                var hasSubject = subjectDic.TryGetValue(item.SubjectId,
                    out var subject);
                var paidAmount = item.CurrencyRate == 0 ? 0 : Math.Round(paidNativeAmount /
                                    item.CurrencyRate, roundScale);
                return new ReceivablePayableDetail
                {
                    SourceId = item.Id,
                    SubjectId = item.SubjectId,
                    DocNo = item.DocNo,
                    VoucherDate = obj.VoucherDate,
                    ForeignAmount = item.ForeignAmount,
                    DebitorCreditor = item.DebitorCreditor,
                    CurrencyCode = item.CurrencyCode,
                    CurrencyRate = item.CurrencyRate,
                    NativeAmount = item.NativeAmount,
                    PaidAmount = paidAmount,
                    PaidNativeAmount = paidNativeAmount,
                    CurrentPaid = paidAmount,
                    NativeCurrentPaid = paidNativeAmount,
                    SubjectCategoryCode = null,
                    AccType = hasSubject == true ?
                        subject.AccountType.Code : string.Empty,
                    AccTypeCategory = hasSubject == true ?
                        subject.AccountType.Category : AccountTypeTypes.Normal,
                    OsAmount = item.ForeignAmount - paidAmount,
                    DueDate = item.DueDate
                };
            });
            return list;
        }
        protected async Task<Dictionary<Guid, Subject>> GetSubjectsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var subjectRepository = LazyServiceProvider.GetRequiredService<ISubjectRepository>();
            var subjects = await subjectRepository.GetPagedListAsync(new SubjectFilterRequest
            {
                SubjectIds = ids,
                IsIncludeAccountType = true
            }, cancellationToken: cancellationToken);
            return subjects.ToDictionary(item => item.Id, item => item);
        }
        public async Task<IEnumerable<ReceivablePayableDetail>> GetReceivableDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var voucher = await GetAsync(id, true, cancellationToken);
            var docNos = voucher.Details.Where((item => item.SubSubjectCode != null))
                .Select(item => item.DocNo).Distinct();
            var creditorId = voucher.Details.FirstOrDefault(item =>
                item.SubSubjectCode != null)?.SubSubjectCode;

            var queryable = await GetReceivablePayableQueryableAsync(creditorId);

            var receivedDic = voucher.Details.ToDictionary(item => item.DocNo,
                item => item.NativeAmount);

            var list = await GetReceivableDetailsAsync(queryable, docNos,
                receivedDic, cancellationToken);

            return list;
        }
        public async Task<long> GetReceivableDetailsCountAsync(Guid? creditorId = null, CancellationToken cancellationToken = default)
        {
            var queryable = await GetReceivablePayableQueryableAsync(creditorId);

            var notReceivedQueryable = queryable
               .GroupBy(item => new { item.DocNo })
               .Where(grp => grp.Sum(item =>
                       item.NativeAmount * (int)item.DebitorCreditor) > 0);
            return await notReceivedQueryable.CountAsync(cancellationToken);
        }

        public async Task<IEnumerable<ReceivablePayableDetail>> GetPayableDetailsAsync(Guid? debitorId, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var detailQueryable = await GetReceivablePayableQueryableAsync(debitorId);

            var notReceivedQueryable = detailQueryable
                .GroupBy(item => new { item.DocNo })
                .Where(grp => grp.Sum(item =>
                        item.NativeAmount * (-(int)item.DebitorCreditor)) > 0)
                .Select(grp => new
                {
                    grp.FirstOrDefault(item => item.IsOriginal == true).Voucher.VoucherDate,
                    grp.Key.DocNo,
                    PaidNativeAmount = grp.Sum(item => item.IsOriginal == false ?
                        item.NativeAmount * (int)item.DebitorCreditor : 0),
                });

            var pageQuerable = notReceivedQueryable
                .OrderBy(item => item.VoucherDate)
                .ThenBy(item => item.DocNo)
               .Skip(skipCount)
               .Take(maxResultCount);

            var notReceiveList = await pageQuerable.ToListAsync(cancellationToken);
            var docNos = notReceiveList.Select(item => item.DocNo);

            var notReceivedDic = notReceiveList.ToDictionary(
                    item => item.DocNo, item => item.PaidNativeAmount);
            var list = await GetPayableDetailsAsync(detailQueryable, docNos, notReceivedDic, [], cancellationToken);

            return list;
        }
        public async Task<IEnumerable<ReceivablePayableDetail>> GetPayableDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var voucher = await GetAsync(id, true, cancellationToken);
            var docNos = voucher.Details.Where((item => item.SubSubjectCode != null))
                .Select(item => item.DocNo).Distinct();
            var debitorId = voucher.Details.FirstOrDefault(item =>
                item.SubSubjectCode != null)?.SubSubjectCode;

            var queryable = await GetReceivablePayableQueryableAsync(debitorId);

            var receivedDic = voucher.Details.GroupBy(item => new { item.DocNo })
                 .Select(grp => new
                 {
                     grp.Key.DocNo,
                     NativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
                 })
                .ToDictionary(item => item.DocNo,
                item => item.NativeAmount);

            var notReceivedQueryable = queryable
               .Where(item => docNos.Contains(item.DocNo))
               .Where(item => item.IsOriginal == false)
               .GroupBy(item => new { item.DocNo })
               .Select(grp => new
               {
                   grp.Key.DocNo,
                   PaidNativeAmount = grp.Sum(item => item.NativeAmount * (int)item.DebitorCreditor),
               });
            var notReceiveList = await notReceivedQueryable.ToListAsync(cancellationToken);
            var notReceivedDic = notReceiveList.ToDictionary(
                    item => item.DocNo, item => item.PaidNativeAmount);

            return await GetPayableDetailsAsync(queryable, docNos, notReceivedDic, receivedDic, cancellationToken);
        }
        public async Task<long> GetPayableDetailsCountAsync(Guid? debitorId = null, CancellationToken cancellationToken = default)
        {
            var detailQueryable = await GetReceivablePayableQueryableAsync(debitorId);

            var notReceivedQueryable = detailQueryable
                .GroupBy(item => new { item.DocNo })
                .Where(grp => grp.Sum(item =>
                        item.NativeAmount * (-(int)item.DebitorCreditor)) > 0);
            return await notReceivedQueryable.CountAsync(cancellationToken);
        }
        private async Task<IEnumerable<ReceivablePayableDetail>> GetPayableDetailsAsync(
           IQueryable<VoucherDetail> queryable, IEnumerable<string> docNos,
           Dictionary<string, decimal> totalReceiveds, Dictionary<string, decimal> currentReceiveds,
              CancellationToken cancellationToken = default)
        {
            var detailsQueryable = queryable.
                                    Where(obj => docNos.Contains(obj.DocNo)
                                        && obj.IsOriginal == true)
                                    .Select(item => new { item, item.Voucher.VoucherDate });

            var details = await detailsQueryable.ToListAsync(cancellationToken);

            var subjectIds = details.Select(item => item.item.SubjectId).Distinct();
            var subjectDic = await GetSubjectsAsync(subjectIds, cancellationToken);

            var roundScale = AccountingCommonConsts.AmountRoundScale;
            var list = details.Select(obj =>
            {
                var item = obj.item;
                totalReceiveds.TryGetValue(item.DocNo, out decimal totalPaidNativeAmount);
                currentReceiveds.TryGetValue(item.DocNo, out decimal currentPaidNativeAmount);

                var hasSubject = subjectDic.TryGetValue(item.SubjectId,
                    out var subject);
                var totalPaidAmount = item.CurrencyRate == 0 ? 0 : Math.Round(totalPaidNativeAmount /
                                    item.CurrencyRate, roundScale);
                var currentPaidAmount = item.CurrencyRate == 0 ? 0 : Math.Round(currentPaidNativeAmount /
                                    item.CurrencyRate, roundScale);

                return new ReceivablePayableDetail
                {
                    SourceId = item.Id,
                    SubjectId = item.SubjectId,
                    DocNo = item.DocNo,
                    VoucherDate = obj.VoucherDate,
                    ForeignAmount = item.ForeignAmount,
                    DebitorCreditor = item.DebitorCreditor,
                    CurrencyCode = item.CurrencyCode,
                    CurrencyRate = item.CurrencyRate,
                    NativeAmount = item.NativeAmount,
                    PaidAmount = totalPaidAmount - currentPaidAmount,
                    PaidNativeAmount = totalPaidNativeAmount - currentPaidNativeAmount,
                    CurrentPaid = currentPaidAmount,
                    NativeCurrentPaid = currentPaidNativeAmount,
                    SubjectCategoryCode = null,
                    AccType = hasSubject == true ?
                        subject.AccountType.Code : string.Empty,
                    AccTypeCategory = hasSubject == true ?
                        subject.AccountType.Category : AccountTypeTypes.Normal,
                    OsAmount = item.ForeignAmount - totalPaidAmount,
                    DueDate = item.DueDate
                };
            });
            return list;
        }

        public async Task<IEnumerable<DocNoRepeatResult>> GetDocNoRepeatsAsync(
              IEnumerable<string>? docNos,
              Guid voucherId,
              CancellationToken cancellationToken = default)
        {
            var queryable = (await GetDbSetAsync()).AsQueryable();
            queryable = queryable.Where(new NoVoidVoucherSpecification());
            var repeatQuery = queryable.Where(obj =>
                    obj.VoucherType == VoucherType.JournalVoucher
                    && obj.Id != voucherId
                ).SelectMany(item => item.Details)
                .Where(item =>
                    docNos.Contains(item.DocNo)
                    && (item.Subject.AccountType.Category == AccountTypeTypes.Receivable
                    || item.Subject.AccountType.Category == AccountTypeTypes.Payable)
                ).GroupBy(item => new
                {
                    item.DocNo,
                    item.SubSubjectCode,
                    AccountTypeCode = item.Subject.AccountType.Code,
                    item.Subject.AccountType.Category
                })
               .Select(item => new DocNoRepeatResult
               {
                   DocNo = item.Key.DocNo,
                   SubSubjectCode = item.Key.SubSubjectCode,
                   AccountTypeCode = item.Key.AccountTypeCode,
                   Category = item.Key.Category,
                   Count = item.Count()
               });
            return await repeatQuery.ToListAsync(cancellationToken);
        }

        protected async Task<IQueryable<Voucher>> GetQueryableWithDetailsAsync()
        {
            var queryable = await WithDetailsAsync(item => item.Details);
            return queryable.Where(new NoVoidVoucherSpecification().ToExpression());
        }

        public async Task<IEnumerable<GeneralLedgerSingleCurrencyReportResult>> GetGLSingleCurrencyListAsync(
            DateOnly startDate, DateOnly endDate, DateOnly periodStartDate, DateOnly periodEndDate,
            Guid? subjectId = null, CancellationToken cancellationToken = default)
        {
            var reposity = new GeneralLedgerReportRepository(await GetQueryableWithDetailsAsync());
            return await reposity.GetGLSingleCurrencyListAsync(startDate, endDate,
                periodStartDate, periodEndDate, subjectId, cancellationToken);
        }
        public async Task<IEnumerable<GeneralLedgerMultipleCurrencyReportResult>> GetGLMultipleCurrencyListAsync(
            DateOnly startDate, DateOnly endDate, DateOnly periodStartDate, DateOnly periodEndDate,
            Guid? subjectId = null, bool isGroup = false, CancellationToken cancellationToken = default)
        {
            var reposity = new GeneralLedgerReportRepository(await GetQueryableWithDetailsAsync());
            return await reposity.GetGLMultipleCurrencyListAsync(startDate, endDate,
                periodStartDate, periodEndDate, subjectId, isGroup, cancellationToken);
        }

        public async Task<IEnumerable<JournalReportSingleCurrencyResult>> GetJLSingleCurrencyListAsync(JournalReportRequest request, string? sorting = null, CancellationToken cancellationToken = default)
        {
            var reposity = new JournalReportRepository(await GetQueryableWithDetailsAsync());

            return await reposity.GetJLSingleCurrencyListAsync(request, sorting, cancellationToken);
        }

        public async Task<IEnumerable<JournalReportMultipleCurrencyResult>> GetJLMultipleCurrencyListAsync(JournalReportRequest request, string? sorting = null, CancellationToken cancellationToken = default)
        {
            var reposity = new JournalReportRepository(await GetQueryableWithDetailsAsync());

            return await reposity.GetJLMultipleCurrencyListAsync(request, sorting, cancellationToken);
        }
    }
}
