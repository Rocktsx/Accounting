using Accounting.Finance.Subjects;
using AutoMapper;
using Magicodes.ExporterAndImporter.Core; 
using System.ComponentModel.DataAnnotations; 

namespace Accounting.Models
{
    [AutoMap(typeof(SubjectImportDto),ReverseMap = true)]
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

        [ImporterHeader(Name = "SubjectCategoryCode")]
        public string? SubjectCategoryCode { get; set; }

        [ImporterHeader(Name = "AccountTypeCode")]
        public string? AccountTypeCode { get; set; }

        [ImporterHeader(Name = "DebitorCreditor")]
        public int DebitorCreditor { get; set; }

        [ImporterHeader(Name = "CurrencyCode")]
        public string? CurrencyCode { get; set; }

        [ImporterHeader(Name = "Description")]
        public string? Description { get; set; }

        [ImporterHeader(Name = "IsSubSubjectType")]
        public bool IsSubSubjectType { get; set; }

        [ImporterHeader(Name = "IsActive")]
        public bool IsActive { get; set; }

        [ImporterHeader(Name = "IsPayMethod")]
        public bool IsPayMethod { get; set; }

        [ImporterHeader(Name = "SeqCode")]
        public int? SeqCode { get; set; }
    }
}
