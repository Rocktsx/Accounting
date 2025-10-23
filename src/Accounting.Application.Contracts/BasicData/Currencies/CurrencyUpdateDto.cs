using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Accounting.BasicData.Currencies
{
    public class CurrencyUpdateDto: IHasConcurrencyStamp
    {
        [Required]
        public decimal SourceAmount { get; set; }
        [Required]
        public decimal TargetAmount { get; set; }
        [Required]
        public decimal ExchangeRate { get; set; }
        public DateOnly? EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public string ConcurrencyStamp { get; set; }
    }
}
