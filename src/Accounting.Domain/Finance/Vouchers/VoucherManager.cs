using Accounting.Common;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.Subjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.Finance.Vouchers
{
    public class VoucherManager : DomainService
    {
        private readonly IAccountingPeriodRepository _accountingPeriodRepository;
        private string _voucherDateFormat;
        public VoucherManager(IAccountingPeriodRepository accountingPeriodRepository)
        {
            _accountingPeriodRepository = accountingPeriodRepository;
        }

        public async Task ValidateAsync(Voucher voucher)
        {
            voucher.ValidateBalance();
            await ValidateVoucherDateAsync(voucher);
            if (voucher.VoucherType == VoucherType.JournalVoucher)
            {
                await ValidateReceivablePayableSubject(voucher);
            }
        }
        private async Task ValidateVoucherDateAsync(Voucher voucher)
        {
            var list = await _accountingPeriodRepository.GetCurrentPeriodsAsync();
            var currentPeriod = list.
                GroupBy(item => item.IsCurrentPeriod).Select(grp => new
                {
                    StartDate = grp.Min(x => x.StartDate),
                    EndDate = grp.Max(x => x.EndDate)
                }).FirstOrDefault();

            var isExists = currentPeriod != null
                && currentPeriod.StartDate <= voucher.VoucherDate
                && voucher.VoucherDate <= currentPeriod.EndDate ? true : false;
            if (isExists == false)
            {
                throw new BusinessException(AccountingDomainErrorCodes.VoucherDateIsNotInCurrentPeriodRange);
            }
        }
        private async Task ValidateReceivablePayableSubject(Voucher voucher)
        {
            var subjectIds = voucher.Details.Select(item => item.SubjectId).Distinct().ToList();
            var subjectRepository = LazyServiceProvider.LazyGetRequiredService<ISubjectRepository>();
            var list = await subjectRepository.GetPagedListAsync(new SubjectFilterRequest { IsIncludeAccountType = true });

            var subjects = list.Where(item => subjectIds.Contains(item.Id)
                && item.AccountType != null &&
                (item.AccountType.Category == AccountTypeTypes.Receivable
                || item.AccountType.Category == AccountTypeTypes.Payable))
                .Select(item => new SimpleSubject
                {
                    Id = item.Id,
                    IsSubSubjectType = item.IsSubSubjectType,
                    AccountTypeCode = item.AccountType.Code,
                    Category = item.AccountType.Category
                });

            if (subjects.Count() == 0)
            {
                return;
            }
            var arapSubjects = subjects.ToDictionary(item => item.Id, item => item);

            var arapVoucherDetails = new List<VoucherDetail>();
            var docDics = new Dictionary<string, VoucherDetail>();
            foreach (var item in voucher.Details)
            {
                if (!arapSubjects.ContainsKey(item.SubjectId))
                {
                    continue;
                }
                var subject = arapSubjects[item.SubjectId];
                if (subject.IsSubSubjectType && (item.SubSubjectCode == null
                    || Guid.Empty.Equals(item.SubSubjectCode)))
                {
                    throw new BusinessException(AccountingDomainErrorCodes.SubSubjectCodeCanNotBeEmpty);
                }
                if (string.IsNullOrWhiteSpace(item.DocNo))
                {
                    throw new BusinessException(AccountingDomainErrorCodes.DocNoCanNotBeEmpty);
                }
                if (item.DueDate == null)
                {
                    throw new BusinessException(AccountingDomainErrorCodes.DueDateCanNotBeEmpty);
                }

                if (docDics.TryGetValue(item.DocNo, out VoucherDetail? value)
                    && (subject.Category == AccountTypeTypes.Receivable ||
                    subject.Category == AccountTypeTypes.Payable
                    && item.SubSubjectCode == value.SubSubjectCode))
                {
                    throw new BusinessException(AccountingDomainErrorCodes.DocNoIsDuplicated);
                }
                else
                {
                    docDics[item.DocNo] = item;
                }
                arapVoucherDetails.Add(item);
            }
            await CheckDocNoRepeatAsync(arapVoucherDetails, arapSubjects);
        }
        /// <summary>
        /// 检查DocNo是否已经使用了
        /// AR的DOC. No.一定是唯一的
        /// AP的DOC. No. 同一供應商一定是唯一的
        /// </summary>
        /// <param name="voucherDetails"></param>
        /// <param name="subjects"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private async Task CheckDocNoRepeatAsync(List<VoucherDetail> voucherDetails, Dictionary<Guid, SimpleSubject> subjects)
        {
            var docNos = voucherDetails.Select(item => item.DocNo);
            var repository = LazyServiceProvider.LazyGetRequiredService<IVoucherRepository>();
            var query = await repository.GetQueryableAsync();
            query = query.Where(new NoVoidVoucherSpecification());

            var voucherId = voucherDetails.First().VoucherId;
            var repeatQuery = query.Where(obj =>
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
               .Select(item => new
               {
                   item.Key.DocNo,
                   item.Key.SubSubjectCode,
                   item.Key.AccountTypeCode,
                   item.Key.Category,
                   Count = item.Count()
               });
            var repeatList = await AsyncExecuter.ToListAsync(repeatQuery);
            if (repeatList.Count == 0)
            {
                return;
            }
            var result = from r in repeatList
                         join d in voucherDetails on r.DocNo equals d.DocNo
                         where r.AccountTypeCode == subjects[d.SubjectId].AccountTypeCode
                           && (r.Category == AccountTypeTypes.Receivable ||
                               r.Category == AccountTypeTypes.Payable
                               && r.SubSubjectCode == d.SubSubjectCode)
                         group r by new
                         {
                             r.DocNo,
                             r.AccountTypeCode,
                             SubSubjectCode = r.Category ==
                                AccountTypeTypes.Receivable ?
                                Guid.Empty : r.SubSubjectCode
                         } into grp
                         where grp.Count() > 0
                         select new
                         {
                             grp.Key.DocNo,
                             grp.Key.SubSubjectCode,
                             grp.Key.AccountTypeCode,
                             Count = grp.Sum(g => g.Count)
                         };

            if (result.Any(item => item.Count > 0))
            {
                var repeatDocNos = string.Join(',', result.Where(item => item.Count > 0).Select(item => item.DocNo));
                throw new BusinessException(AccountingDomainErrorCodes.DocNoHasBeenUsed).WithData("DocNos", repeatDocNos);
            }
        }
        public VoucherManager SetVoucherDateFormat(string format)
        {
            _voucherDateFormat = format;
            return this;
        }
        public static string GetPrefix(Voucher voucher, string dateFormat)
        {
            var prefix = voucher.Prefix;
            if (string.IsNullOrWhiteSpace(dateFormat))
            {
                return prefix;
            }
            var datePrefix = voucher.VoucherDate.ToString(dateFormat);
            if (!string.IsNullOrWhiteSpace(datePrefix))
            {
                prefix += "-" + datePrefix;
            }
            return prefix;
        }
        private class SimpleSubject
        {
            public Guid Id { get; set; }
            public string AccountTypeCode { get; set; }
            public AccountTypeTypes Category { get; set; }
            public bool IsSubSubjectType { get; set; }
        }
    }
}
