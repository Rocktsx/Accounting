using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance
{
    /// <summary>
    /// 科目类别
    /// </summary>
    public class SubjectCategory : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string OtherName { get; private set; }
        public Guid? ParentId { get; private set; }
        public CreditDebit CreditDebit { get; private set; }
        public Guid? AccountTypeId { get; private set; }
        public bool ShowDetail { get; private set; }
        public string Description { get; private set; }

        public virtual AccountType AccountType { get; private set; }

        public virtual ICollection<Subject> Subjects { get; private set; }
        private SubjectCategory() { }
        public SubjectCategory(
            Guid id,
            string code,
            string name,
            string otherName,
            Guid? parentId,
            CreditDebit creditDebit,
            Guid? accountTypeId,
            bool showDetail,
            string description
        ) : base(id)
        {
            SetCode(code);
            SetName(name);
            SetOtherName(otherName);
            SetParentId(parentId);
            SetCreditDebit(creditDebit);
            SetAccountTypeId(accountTypeId);
            SetShowDetail(showDetail);
            SetDescription(description);
        }

        public SubjectCategory SetCode(string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Code = code;
            return this;
        }
        public SubjectCategory SetName(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Name = name;
            return this;
        }
        public SubjectCategory SetOtherName(string otherName)
        {
            OtherName = otherName??string.Empty;
            return this;
        }
        public SubjectCategory SetParentId(Guid? parentId)
        {
            ParentId = parentId;
            return this;
        }
        public SubjectCategory SetCreditDebit(CreditDebit creditDebit)
        {
            CreditDebit = creditDebit;
            return this;
        }
        public SubjectCategory SetAccountTypeId(Guid? accountTypeId)
        { 
            AccountTypeId = accountTypeId;
            return this;
        }
        public SubjectCategory SetShowDetail(bool showDetail)
        {
            ShowDetail = showDetail;
            return this;
        }
        public SubjectCategory SetDescription(string description)
        {
            Description = description ?? string.Empty;
            return this;
        }
    }
}
