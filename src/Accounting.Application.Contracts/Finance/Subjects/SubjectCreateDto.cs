using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Subjects
{
    public class SubjectCreateDto
    {
        [Required]
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        public string Code { get; set; }
        [Required]
        [MaxLength(AccountingCommonConsts.MaxNameLength)]
        public string Name { get; set; } 
        [MaxLength(AccountingCommonConsts.MaxNameLength)]
        public string OtherName { get; set; }
        public Guid? SubjectCategoryId { get; set; }
        public Guid? AccountTypeId { get; set; }
        public DebitorCreditor DebitorCreditor { get; set; }
        [MaxLength(AccountingCommonConsts.MaxCodeLength)]
        public string CurrencyCode { get; set; }
        [MaxLength (AccountingCommonConsts.MaxDescriptionLength)]
        public string Description { get; set; }
        public bool IsSubSubjectType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPayMethod { get; set; }
        public int? SeqCode { get; set; }

        public SubjectCreateDto()
        {
            Code = string.Empty;
            Name = string.Empty;
            OtherName = string.Empty;
            Description = string.Empty;
            CurrencyCode = string.Empty;
        }
    }
}
