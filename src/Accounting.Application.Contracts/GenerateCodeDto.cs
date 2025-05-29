
using System.ComponentModel.DataAnnotations;

namespace Accounting
{
    public class GenerateCodeDto
    {
        public string Code { get; set; }
        [Required]
        [MaxLength(AccountingCommonConsts.MaxPrefixLength)]
        public string Prefix { get; set; }
        public int GenNo { get; set; } 
    }
}
