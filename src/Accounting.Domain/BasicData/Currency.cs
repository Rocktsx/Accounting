using System; 
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Accounting.BasicData
{
    /// <summary>
    ///货币
    /// </summary>
    public class Currency : AuditedEntity<Guid>, IMultiTenant
    { 
        public string SourceCurrency {get; private set; }
        public string TargetCurrency { get; private set; }
        public decimal SourceAmount { get; private set; }
        public decimal TargetAmount { get; private set; }
        public decimal ExchangeRate { get; private set; }
        public DateOnly? EffectiveDate { get; private set; }
        public bool IsActive { get; private set; }

        public Guid? TenantId { get; private set; }

        private Currency() { } // For EF Core
        public Currency(Guid id, string sourceCurrency, string targetCurrency, decimal sourceAmount, decimal targetAmount, 
            decimal exchangeRate, DateOnly? effectiveDate, bool isActive, Guid? tenantId = null)
        {
            Id = id;
            SetCurrency(sourceCurrency, targetCurrency);
             
            SetAmountAndRate(sourceAmount, targetAmount, exchangeRate); 
            EffectiveDate = effectiveDate;
            IsActive = isActive;
            TenantId = tenantId;
        }
        public void SetCurrency(string sourceCurrency, string targetCurrency)
        {
            Check.NotNullOrWhiteSpace(sourceCurrency, nameof(sourceCurrency), CurrencyConsts.MaxCurrencyLength);
            Check.NotNullOrWhiteSpace(targetCurrency, nameof(targetCurrency), CurrencyConsts.MaxCurrencyLength);
           
            SourceCurrency = sourceCurrency;
            TargetCurrency = targetCurrency;
        }
        public Currency SetAmountAndRate(decimal sourceAmount, decimal targetAmount, decimal rate)
        {
            if (sourceAmount <= 0 || targetAmount <= 0 || rate <= 0)
            {
                throw new BusinessException(AccountingDomainErrorCodes.AmountsMustNotBeZero);
            }
            if (sourceAmount / targetAmount != rate)
            {
                throw new BusinessException(AccountingDomainErrorCodes.ExchangeRateMatchAmounts);
            }
            SourceAmount = sourceAmount;
            TargetAmount = targetAmount;
            ExchangeRate = rate;

            return this;
        }
        public Currency SetEffectiveDate(DateOnly? effectiveDate)
        {
            EffectiveDate = effectiveDate;

            return this;
        }
        public Currency SetIsActive(bool isActive)
        {
            IsActive = isActive;
            return this;
        }
    }
}
