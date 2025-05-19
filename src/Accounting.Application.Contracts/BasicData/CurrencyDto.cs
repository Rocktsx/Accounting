using System; 

namespace Accounting.BasicData
{
    public class CurrencyDto
    {
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal SourceAmount { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}
