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
        [ImporterHeader(Name = "ParentCode")]
        public string? ParentCode { get; set; }
        [ImporterHeader(Name = "DebitorCreditor")]
        public int DebitorCreditor { get; set; }
        [ImporterHeader(Name = "AccountTypeCode")]
        public string? AccountTypeCode { get; set; }
        [ImporterHeader(Name = "ShowDetail")]
        public bool ShowDetail { get; set; }
        [ImporterHeader(Name = "Description")]
        public string? Description { get; set; }
        [ImporterHeader(Name = "Level")]
        public int Level { get; set; }
    }
}
