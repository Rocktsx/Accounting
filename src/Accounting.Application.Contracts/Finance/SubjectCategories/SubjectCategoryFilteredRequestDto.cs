using Accounting.Dtos;

namespace Accounting.Finance.SubjectCategories
{
    public class SubjectCategoryFilteredRequestDto: FilteredPagedAndSortedResultRequestDto
    {
        public bool? IsIncludeAccountType { get; set; }
        public bool? IsIncludeParent { get; set; }
    }
}
