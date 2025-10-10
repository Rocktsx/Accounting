using Accounting.Finance.SubjectCategories;
using AutoMapper;
using Magicodes.ExporterAndImporter.Core;

namespace Accounting.Models
{
    [AutoMap(typeof(SubjectCategoryImportDto), ReverseMap =true)]     
    
    public class SubjectCategoryImportModel
    {
        [ImporterHeader(Name = "Code")]
        public string? Code { get; set; }

        [ImporterHeader(Name = "Name")]
        public string? Name { get; set; }

        [ImporterHeader(Name = "OtherName")]
        public string? OtherName { get; set; }

        [ImporterHeader(Name = "Parent")]
        public string? ParentCode { get; set; }

        [ImporterHeader(Name = "DebitorCreditor")]
        [ValueMapping("DR", 1)]
        [ValueMapping("CR", -1)]
        public int DebitorCreditor { get; set; }

        [ImporterHeader(Name = "AccountType")]
        public string? AccountTypeCode { get; set; }

        [ImporterHeader(Name = "ShowDetail")]
        [ValueMapping("1", true)] 
        public bool ShowDetail { get; set; }

        [ImporterHeader(Name = "Description")]
        public string? Description { get; set; }

        [ImporterHeader(Name = "Level")]
        public int Level { get; set; }
    }
}
