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

namespace Accounting.Finance
{
    public class VoucherManager : GenerateCodeService
    {
        private readonly IRepository<AccountingPeriod, Guid> _accountingPeriodRepository;
        private string _voucherDateFormat;
        public VoucherManager(IRepository<AccountingPeriod, Guid> accountingPeriodRepository)
        {
            _accountingPeriodRepository = accountingPeriodRepository;
        }

        public async Task ValidateAsync(Voucher voucher)
        {
            ValidateBalance(voucher);
            await ValidateVoucherDateAsync(voucher);
            if (voucher.VoucherType == VoucherType.JournalVoucher)
            {
                await ValidateReceivablePayableSubject(voucher);
            }
        }
        public static void SetFunctionalFields(VoucherDetail item, bool enableProjectFunction, bool enableRegionFunction,
            bool enableDepartmentFunction, bool enableCustom1Function, bool enableCustom2Function, string project,
            string region, string department, string custom1, string custom2)
        {
            if (item == null)
            {
                return;
            }
            if (enableProjectFunction == true)
            {
                item.SetProject(project);
            }
            if (enableRegionFunction == true)
            {
                item.SetRegion(region);
            }
            if (enableDepartmentFunction == true)
            {
                item.SetDepartment(department);
            }
            if (enableCustom1Function == true)
            {
                item.SetCustom1(custom1);
            }
            if (enableCustom2Function == true)
            {
                item.SetCustom2(custom2);
            }
        }
        private void ValidateBalance(Voucher voucher)
        {
            var amount = voucher.Details.Sum(item => item.NativeAmount * (int)item.DebitorCreditor);
            if (amount != 0)
            {
                throw new BusinessException(AccountingDomainErrorCodes.VoucherDoesNotBalance);
            }
        }
        private async Task ValidateVoucherDateAsync(Voucher voucher)
        {
            var queryable = await _accountingPeriodRepository.GetQueryableAsync(); 
            var periodQueryable = queryable.Where(item => item.IsCurrentPeriod).GroupBy(item => item.IsCurrentPeriod).Select(grp => new
            {
                StartDate = grp.Min(x => x.StartDate),
                EndDate = grp.Max(x => x.EndDate)
            });
            var currentPeriod = await AsyncExecuter.FirstOrDefaultAsync(periodQueryable);
            var isExists = currentPeriod != null && currentPeriod.StartDate <= voucher.VoucherDate && voucher.VoucherDate <= currentPeriod.EndDate ? true : false;
            if (isExists == false)
            {
                throw new BusinessException(AccountingDomainErrorCodes.VoucherDateIsNotInCurrentPeriodRange);
            }
        }
        private async Task ValidateReceivablePayableSubject(Voucher voucher)
        {
            var subjectIds = voucher.Details.Select(item => item.SubjectId).Distinct().ToList();
            var subjectRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<Subject, Guid>>();
            var querable = await subjectRepository.WithDetailsAsync(item => item.AccountType);

            var arapQuerable = querable.Where(item => subjectIds.Contains(item.Id) && item.AccountType != null &&
                (item.AccountType.Code == AccountTypeConsts.AccountingReceivableType || item.AccountType.Code == AccountTypeConsts.AccountingPayableType))
                .Select(item => new SimpleSubject { Id = item.Id, IsSubSubjectType = item.IsSubSubjectType, AccountTypeCode = item.AccountType.Code });
            var subjects = await AsyncExecuter.ToListAsync(arapQuerable);

            if (subjects.Count == 0)
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
                if (subject.IsSubSubjectType && (item.SubSubjectCode == null || Guid.Empty.Equals(item.SubSubjectCode)))
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

                if (docDics.TryGetValue(item.DocNo, out VoucherDetail? value) && (subject.AccountTypeCode == AccountTypeConsts.AccountingReceivableType ||
                    subject.AccountTypeCode == AccountTypeConsts.AccountingPayableType && item.SubSubjectCode == value.SubSubjectCode))
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
            var repository = LazyServiceProvider.LazyGetRequiredService<IRepository<VoucherDetail, Guid>>();
            var query = await repository.GetQueryableAsync();
            var voucherId = voucherDetails.First().VoucherId;
            var docNos = voucherDetails.Select(item => item.DocNo);

            var repeatQuery = query.Where(item => item.Voucher.VoucherType == VoucherType.JournalVoucher
                                        && item.VoucherId != voucherId
                                        && docNos.Contains(item.DocNo)
                                        && (item.Subject.AccountType.Code == AccountTypeConsts.AccountingReceivableType
                                            || item.Subject.AccountType.Code == AccountTypeConsts.AccountingPayableType)
                                  ).GroupBy(item => new { item.DocNo, item.SubSubjectCode, AccountTypeCode = item.Subject.AccountType.Code })
                                  .Select(item => new { item.Key.DocNo, item.Key.SubSubjectCode, item.Key.AccountTypeCode, Count = item.Count() });
            var repeatList = await AsyncExecuter.ToListAsync(repeatQuery);
            if (repeatList.Count == 0)
            {
                return;
            }
            var result = from r in repeatList
                         join d in voucherDetails on r.DocNo equals d.DocNo
                         where r.AccountTypeCode == subjects[d.SubjectId].AccountTypeCode
                           && (r.AccountTypeCode == AccountTypeConsts.AccountingReceivableType ||
                               r.AccountTypeCode == AccountTypeConsts.AccountingPayableType && r.SubSubjectCode == d.SubSubjectCode)
                         group r by new { r.DocNo, r.AccountTypeCode, SubSubjectCode = (r.AccountTypeCode == AccountTypeConsts.AccountingReceivableType ? Guid.Empty : r.SubSubjectCode) } into grp
                         where grp.Count() > 0
                         select new { grp.Key.DocNo, grp.Key.SubSubjectCode, grp.Key.AccountTypeCode, Count = grp.Sum(g => g.Count) };

            if (result.Any(item => item.Count > 0))
            {
                var repeatDocNos = String.Join(',', result.Where(item => item.Count > 0).Select(item => item.DocNo));
                throw new BusinessException(AccountingDomainErrorCodes.DocNoHasBeenUsed).WithData("DocNos", repeatDocNos);
            }
        }
        public VoucherManager SetVoucherDateFormat(string format)
        {
            _voucherDateFormat = format;
            return this;
        }
        protected override string GetPrefix(IGenerateCode obj)
        {
            if (string.IsNullOrWhiteSpace(_voucherDateFormat))
            {
                return base.GetPrefix(obj);
            }
            var prefix = base.GetPrefix(obj);
            try
            {
                var datePrefix = (obj as Voucher).VoucherDate.ToString(_voucherDateFormat);
                if (!string.IsNullOrWhiteSpace(datePrefix))
                {
                    prefix += "-" + datePrefix;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(AccountingDomainErrorCodes.CannotFormatVoucherDate, innerException: ex).WithData("Format", _voucherDateFormat);
            }
            return prefix;
        }
        private class SimpleSubject
        {
            public Guid Id { get; set; }
            public string AccountTypeCode { get; set; }
            public bool IsSubSubjectType { get; set; }
        }
    }
}
