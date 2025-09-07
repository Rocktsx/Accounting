using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.Finance
{
    public class VoucherManager : DomainService
    {
        private readonly IRepository<AccountingPeriod, Guid> _accountingPeriodRepository;

        public VoucherManager(IRepository<AccountingPeriod, Guid> accountingPeriodRepository)
        {
            _accountingPeriodRepository = accountingPeriodRepository;
        }

        public async Task ValidateAsync(Voucher voucher)
        {
            ValidateBalance(voucher);
            await ValidateVoucherDateAsync(voucher);
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
            var periodQueryable = queryable.GroupBy(item => item.IsCurrentPeriod).Where(item => item.Key == true &&
                item.Min(x => x.StartDate) > voucher.VoucherDate || item.Max(x => x.EndDate) < voucher.VoucherDate);
            var isExists = await AsyncExecuter.AnyAsync(periodQueryable);
            if (isExists)
            {
                throw new BusinessException(AccountingDomainErrorCodes.VoucherDateIsNotInCurrentPeriodRange);
            }
        }
        public async Task ValidateReceivablePayableSubject(Voucher voucher, IRepository<Subject, Guid> subjectRepository)
        {
            var subjectIds = voucher.Details.Select(item => item.SubjectId).Distinct().ToList();
            var querable = await subjectRepository.WithDetailsAsync(item => item.AccountType);
            var arapQuerable = querable.Where(item => subjectIds.Contains(item.Id) && item.AccountType != null && 
                (item.AccountType.Code == AccountTypeConsts.AccountingReceivableType || item.AccountType.Code == AccountTypeConsts.AccountingPayableType))
                .Select(item => new { item.Id, item.IsSubSubjectType }); 
            var arapSubjects = (await AsyncExecuter.ToListAsync(arapQuerable)).ToDictionary(item => item.Id, item => item.IsSubSubjectType);

            foreach (var item in voucher.Details)
            {
                if (!arapSubjects.ContainsKey(item.SubjectId))
                {
                    continue;
                }
                if (arapSubjects[item.SubjectId] && string.IsNullOrWhiteSpace(item.SubSubjectCode))
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
            }
        }
    }
}
