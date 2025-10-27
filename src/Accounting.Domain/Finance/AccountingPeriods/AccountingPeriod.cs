using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance.AccountingPeriods
{
    /// <summary>
    /// 会计年度
    /// </summary>
    public class AccountingPeriod : AuditedEntity<Guid>, IMultiTenant, IHasConcurrencyStamp
    {
        public Guid? TenantId { get; private set; }
        public string Code { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public bool IsCurrentPeriod { get; private set; }
        public string ConcurrencyStamp { get; set; }
        private AccountingPeriod() { }

        public AccountingPeriod(Guid id, string code, DateOnly startDate, DateOnly endDate, bool isCurrentPeriod, Guid? tenantId = null)
        {
            Id = id;
            SetCode(code);
            SetStartDate(startDate);
            SetEndDate(endDate);
            SetIsCurrentPeriod(isCurrentPeriod);
            TenantId = tenantId;
        }
        public AccountingPeriod SetCode(string code)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Code = code;
            return this;
        }
        public AccountingPeriod SetStartDate(DateOnly startDate)
        {
            StartDate = startDate;
            return this;
        }
        public AccountingPeriod SetEndDate(DateOnly endDate)
        {
            EndDate = endDate;
            return this;
        }
        public AccountingPeriod SetIsCurrentPeriod(bool isCurrentPeriod)
        {
            IsCurrentPeriod = isCurrentPeriod;
            return this;
        }
    }
}
