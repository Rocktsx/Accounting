using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.Settings;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Web.Pages.BasicData.Currencies;
using Accounting.Web.ViewModels;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Accounting.Web;

#region Currency

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateEditCurrencyViewModelToCurrencyCreateDtoMapper : MapperBase<CreateEditCurrencyViewModel, CurrencyCreateDto>
{
    public override partial CurrencyCreateDto Map(CreateEditCurrencyViewModel source);

    public override partial void Map(CreateEditCurrencyViewModel source, CurrencyCreateDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateEditCurrencyViewModelToCurrencyUpdateDtoMapper : MapperBase<CreateEditCurrencyViewModel, CurrencyUpdateDto>
{
    public override partial CurrencyUpdateDto Map(CreateEditCurrencyViewModel source);

    public override partial void Map(CreateEditCurrencyViewModel source, CurrencyUpdateDto destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CurrencyDtoToCreateEditCurrencyViewModelMapper : MapperBase<CurrencyDto, CreateEditCurrencyViewModel>
{
    public override partial CreateEditCurrencyViewModel Map(CurrencyDto source);

    public override partial void Map(CurrencyDto source, CreateEditCurrencyViewModel destination);
}
#endregion

#region Company

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CompanyDtoToCreateOrEditCompanyViewModelMapper : MapperBase<CompanyDto, CreateOrEditCompanyViewModel>
{
    public override partial CreateOrEditCompanyViewModel Map(CompanyDto source);

    public override partial void Map(CompanyDto source, CreateOrEditCompanyViewModel destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CompanyAddressDtoToCreateOrEditCompanyAddressViewModelMapper : MapperBase<CompanyAddressDto, CreateOrEditCompanyAddressViewModel>
{
    public override partial CreateOrEditCompanyAddressViewModel Map(CompanyAddressDto source);

    public override partial void Map(CompanyAddressDto source, CreateOrEditCompanyAddressViewModel destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CompanyContactDtoToCreateOrEditCompanyContactViewModelMapper : MapperBase<CompanyContactDto, CreateOrEditCompanyContactViewModel>
{
    public override partial CreateOrEditCompanyContactViewModel Map(CompanyContactDto source);

    public override partial void Map(CompanyContactDto source, CreateOrEditCompanyContactViewModel destination);
}
#endregion

#region AccountingPeriod

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AccountingPeriodDtoToEditAccountingPeriodViewModelMapper : MapperBase<AccountingPeriodDto, EditAccountingPeriodViewModel>
{
    public override partial EditAccountingPeriodViewModel Map(AccountingPeriodDto source);

    public override partial void Map(AccountingPeriodDto source, EditAccountingPeriodViewModel destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateAccountingPeriodViewModelToAccountingPeriodCreateDtoMapper : MapperBase<CreateAccountingPeriodViewModel, AccountingPeriodCreateDto>
{
    public override partial AccountingPeriodCreateDto Map(CreateAccountingPeriodViewModel source);

    public override partial void Map(CreateAccountingPeriodViewModel source, AccountingPeriodCreateDto destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EditAccountingPeriodViewModelToAccountingPeriodUpdateDtoMapper : MapperBase<EditAccountingPeriodViewModel, AccountingPeriodUpdateDto>
{
    public override partial AccountingPeriodUpdateDto Map(EditAccountingPeriodViewModel source);

    public override partial void Map(EditAccountingPeriodViewModel source, AccountingPeriodUpdateDto destination);
}
#endregion

#region AccountType

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AccountTypeDtoToEditAccountTypeViewModelMapper : MapperBase<AccountTypeDto, EditAccountTypeViewModel>
{
    public override partial EditAccountTypeViewModel Map(AccountTypeDto source);

    public override partial void Map(AccountTypeDto source, EditAccountTypeViewModel destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateAccountTypeViewModelToAccountTypeCreateDtoMapper : MapperBase<CreateAccountTypeViewModel, AccountTypeCreateDto>
{
    public override partial AccountTypeCreateDto Map(CreateAccountTypeViewModel source);

    public override partial void Map(CreateAccountTypeViewModel source, AccountTypeCreateDto destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EditAccountTypeViewModelToAccountTypeUpdateDtoMapper : MapperBase<EditAccountTypeViewModel, AccountTypeUpdateDto>
{
    public override partial AccountTypeUpdateDto Map(EditAccountTypeViewModel source);

    public override partial void Map(EditAccountTypeViewModel source, AccountTypeUpdateDto destination);
}

#endregion

#region SubjectCategory

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SubjectCategoryDtoToEditSubjectCategoryViewModelMapper : MapperBase<SubjectCategoryDto, EditSubjectCategoryViewModel>
{
    public override partial EditSubjectCategoryViewModel Map(SubjectCategoryDto source);

    public override partial void Map(SubjectCategoryDto source, EditSubjectCategoryViewModel destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateSubjectCategoryViewModelToSubjectCategoryCreateDtoMapper : MapperBase<CreateSubjectCategoryViewModel, SubjectCategoryCreateDto>
{
    public override partial SubjectCategoryCreateDto Map(CreateSubjectCategoryViewModel source);

    public override partial void Map(CreateSubjectCategoryViewModel source, SubjectCategoryCreateDto destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EditSubjectCategoryViewModelToSubjectCategoryUpdateDtoMapper : MapperBase<EditSubjectCategoryViewModel, SubjectCategoryUpdateDto>
{
    public override partial SubjectCategoryUpdateDto Map(EditSubjectCategoryViewModel source);

    public override partial void Map(EditSubjectCategoryViewModel source, SubjectCategoryUpdateDto destination);
}

#endregion

#region Subject

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SubjectDtoToEditSubjectViewModelMapper : MapperBase<SubjectDto, EditSubjectViewModel>
{
    public override partial EditSubjectViewModel Map(SubjectDto source);

    public override partial void Map(SubjectDto source, EditSubjectViewModel destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateSubjectViewModelToSubjectCreateDtoMapper : MapperBase<CreateSubjectViewModel, SubjectCreateDto>
{
    public override partial SubjectCreateDto Map(CreateSubjectViewModel source);

    public override partial void Map(CreateSubjectViewModel source, SubjectCreateDto destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EditSubjectViewModelToSubjectUpdateDtoMapper : MapperBase<EditSubjectViewModel, SubjectUpdateDto>
{
    public override partial SubjectUpdateDto Map(EditSubjectViewModel source);

    public override partial void Map(EditSubjectViewModel source, SubjectUpdateDto destination);
}

#endregion AccountingSetting

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AccountingSettingDtoToAccountingSettingViewModelMapper : MapperBase<AccountingSettingDto, AccountingSettingViewModel>
{
    public override partial AccountingSettingViewModel Map(AccountingSettingDto source);

    public override partial void Map(AccountingSettingDto source, AccountingSettingViewModel destination);
}

