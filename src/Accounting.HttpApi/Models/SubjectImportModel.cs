using Magicodes.ExporterAndImporter.Core; 
using System.ComponentModel.DataAnnotations; 

namespace Accounting.Models
{
    internal class SubjectImportModel
    {
        [Required]
        [ImporterHeader(Name = "Code")]
        public string? Code { get; set; }

        [Required]
        [ImporterHeader(Name = "Name")]
        public string? Name { get; set; }

        [ImporterHeader(Name = "OtherName")]
        public string? OtherName { get; set; }

        [ImporterHeader(Name = "SubjectCategory")]
        public string? SubjectCategoryCode { get; set; }

        [ImporterHeader(Name = "AccountType")]
        public string? AccountTypeCode { get; set; }

        [ImporterHeader(Name = "DebitorCreditor")]
        [ValueMapping("DR", 1)]
        [ValueMapping("CR", -1)]
        public int DebitorCreditor { get; set; }

        [ImporterHeader(Name = "Currency")]
        public string? CurrencyCode { get; set; }

        [ImporterHeader(Name = "Description")]
        public string? Description { get; set; }

        [ImporterHeader(Name = "IsSubSubjectType")]
        [ValueMapping("1", true)] 
        public bool IsSubSubjectType { get; set; }

        [ImporterHeader(Name = "IsActive")]
        [ValueMapping("1", true)] 
        public bool IsActive { get; set; }

        [ImporterHeader(Name = "IsPayMethod")]
        [ValueMapping("1", true)] 
        public bool IsPayMethod { get; set; }

        [ImporterHeader(Name = "Seq")]
        public int? SeqCode { get; set; }
    }
}
