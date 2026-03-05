using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Subjects
{
    public class SubjectImportDto
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public string OtherName { get; set; }
        public string SubjectCategoryCode { get; set; }
        public string AccountTypeCode { get; set; }
        public int DebitorCreditor { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public bool IsSubSubjectType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPayMethod { get; set; }
        public int? SeqCode { get; set; }

        public SubjectImportDto()
        {
            Code = string.Empty;
            Name = string.Empty;
            OtherName = string.Empty;
            SubjectCategoryCode = string.Empty;
            AccountTypeCode = string.Empty;
            CurrencyCode = string.Empty;
            Description = string.Empty;
        }
    }
}
