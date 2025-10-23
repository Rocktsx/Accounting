using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Accounting.BasicData.Currencies
{
    public class CurrencyDto :AuditedEntityDto<Guid>, IHasConcurrencyStamp
    { 
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal SourceAmount { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public string ConcurrencyStamp { get; set; }
    }
}
