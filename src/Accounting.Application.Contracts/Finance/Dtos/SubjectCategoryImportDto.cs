
using System.ComponentModel.DataAnnotations;

namespace Accounting.Finance.Dtos
{
    public class SubjectCategoryImportDto
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public string? OtherName { get; set; }
        public string? ParentCode { get; set; }
        public int DebitorCreditor { get; set; }
        public string? AccountTypeCode { get; set; }
        public bool ShowDetail { get; set; }
        public string? Description { get; set; }
        public int Level { get; set; }
    }
}
