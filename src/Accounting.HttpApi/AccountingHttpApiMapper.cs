using Accounting.BasicData.Companies;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Accounting.Models;
using Riok.Mapperly.Abstractions; 
using Volo.Abp.Mapperly;

namespace Accounting
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class CompanyImportModelToCompanyImportDtoMapper : MapperBase<CompanyImportModel, CompanyImportDto>
    {
        public override partial CompanyImportDto Map(CompanyImportModel source);

        public override partial void Map(CompanyImportModel source, CompanyImportDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SubjectCategoryImportModelToSubjectCategoryImportDtoMapper : MapperBase<SubjectCategoryImportModel, SubjectCategoryImportDto>
    {
        public override partial SubjectCategoryImportDto Map(SubjectCategoryImportModel source);

        public override partial void Map(SubjectCategoryImportModel source, SubjectCategoryImportDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    internal partial class SubjectImportModelToSubjectImportDtoMapper : MapperBase<SubjectImportModel, SubjectImportDto>
    {
        public override partial SubjectImportDto Map(SubjectImportModel source);

        public override partial void Map(SubjectImportModel source, SubjectImportDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class VoucherImportItemModelToVoucherImportItemDtoMapper : MapperBase<VoucherImportItemModel, VoucherImportItemDto>
    {
        public override partial VoucherImportItemDto Map(VoucherImportItemModel source);

        public override partial void Map(VoucherImportItemModel source, VoucherImportItemDto destination);
    }

}
