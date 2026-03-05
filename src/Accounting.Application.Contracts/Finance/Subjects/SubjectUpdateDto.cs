using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Accounting.Finance.Subjects
{
    public class SubjectUpdateDto: IHasConcurrencyStamp
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
        [MaxLength(AccountingCommonConsts.MaxDescriptionLength)]
        public string Description { get; set; }
        public bool IsSubSubjectType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPayMethod { get; set; }
        public int? SeqCode { get; set; }

        public string ConcurrencyStamp { get; set; }

        public SubjectUpdateDto()
        {
            Code = string.Empty;
            Name = string.Empty;
            OtherName = string.Empty;
            CurrencyCode = string.Empty;
            Description = string.Empty; 
        }
    }
}
