using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance
{
    /// <summary>
    /// 科目
    /// </summary>
    public class Subject : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string OtherName { get; private set; }
        public Guid? SubjectCategoryId { get; private set; }
        public Guid? AccountTypeId { get; private set; }
        public CreditDebit CreditDebit { get; private set; }
        public string CurrencyCode { get; private set; }
        public string Description { get; private set; }
        public bool IsSubSujectType { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsPayMethod { get; private set; }
        public int? SeqCode { get; private set; }

        public virtual SubjectCategory SubjectCategory { get; private set; }
        public virtual AccountType AccountType { get; private set; }
        private Subject() { }
        public Subject(
            Guid id,
            string code,
            string name,
            string otherName,
            Guid? subjectCategoryId,
            Guid? accountTypeId,
            CreditDebit creditDebit,
            string currencyCode,
            string description,
            bool isSubSujectType,
            bool isActive,
            bool isPayMethod,
            int? seqCode
        ) : base(id)
        {
            SetCode(code);
            SetName(name);
            SetOtherName(otherName);
            SetSubjectCategoryId(subjectCategoryId);
            SetAccountTypeId(accountTypeId);
            SetCreditDebit(creditDebit);
            SetCurrencyCode(currencyCode);
            SetDescription(description);
            SetIsSubSujectType(isSubSujectType);
            SetIsActive(isActive);
            SetIsPayMethod(isPayMethod);
            SetSeqCode(seqCode);
        }
        public Subject SetCode(string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Code = code;
            return this;
        }
        public Subject SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
            return this;
        }
        public Subject SetOtherName(string otherName)
        {
            OtherName = otherName ?? string.Empty;
            return this;
        } 
       
        public Subject SetCreditDebit(CreditDebit creditDebit)
        {
            CreditDebit = creditDebit;
            return this;
        }
        public Subject SetAccountTypeId(Guid? accountTypeId)
        { 
            AccountTypeId = accountTypeId;
            return this;
        }
        public Subject SetDescription(string description)
        {
            Description = description ?? string.Empty;
            return this;
        }
        public Subject SetSeqCode(int? seqCode)
        {
            SeqCode = seqCode;
            return this;
        }

        public Subject SetIsPayMethod(bool isPayMethod)
        {
            IsPayMethod = isPayMethod;
            return this;
        }

        public Subject SetIsActive(bool isActive)
        {
            IsActive = isActive;
            return this;
        }

        public Subject SetIsSubSujectType(bool isSubSujectType)
        {
            IsSubSujectType = isSubSujectType;
            return this;
        }

        public Subject SetCurrencyCode(string currencyCode)
        {
            CurrencyCode =currencyCode??string.Empty;
            return this;
        }

        public Subject SetSubjectCategoryId(Guid? subjectCategoryId)
        {
            SubjectCategoryId = subjectCategoryId;
            return this;
        } 
    }
}
