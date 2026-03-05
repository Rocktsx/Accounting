using System.ComponentModel.DataAnnotations;

namespace Accounting.Dtos
{
    public class GenerateCodeDto
    {
        public string Code { get; set; }
        [Required]
        [MaxLength(AccountingCommonConsts.MaxPrefixLength)]
        public string Prefix { get; set; }
        public int GenNo { get; set; } 

        public GenerateCodeDto()
        {
            Code = string.Empty;
            Prefix = string.Empty;
        }
    }
}
