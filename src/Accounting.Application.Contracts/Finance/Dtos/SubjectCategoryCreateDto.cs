using System; 
using System.ComponentModel.DataAnnotations; 

namespace Accounting.Finance.Dtos
{
    public class SubjectCategoryCreateDto
    {
        [Required]
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        [MaxLength(AccountingCommonConsts.MaxNameLength)]
        public string Name { get; set; }
        [MaxLength(AccountingCommonConsts.MaxNameLength)]
        public string OtherName { get; set; }
        public Guid? ParentId { get; set; }
        public DebitorCreditor DebitorCreditor { get; set; }
        public Guid? AccountTypeId { get; set; }
        public bool ShowDetail { get; set; }
        [MaxLength(AccountingCommonConsts.MaxDescriptionLength)]
        public string Description { get; set; }
    }
}
