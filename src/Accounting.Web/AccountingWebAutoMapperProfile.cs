using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.Settings;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Web.Pages.BasicData.Currencies;
using Accounting.Web.ViewModels;
using AutoMapper;
using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;

namespace Accounting.Web;

public class AccountingWebAutoMapperProfile : Profile
{
    public AccountingWebAutoMapperProfile()
    {
        //Define your object mappings here, for the Web project
        CreateMap<CreateEditCurrencyViewModel, CurrencyCreateDto>();
        CreateMap<CreateEditCurrencyViewModel, CurrencyUpdateDto>();
        CreateMap<CurrencyDto, CreateEditCurrencyViewModel>();

        CreateMap<CompanyDto, CreateOrEditCompanyViewModel>();
        CreateMap<CompanyAddressDto, CreateOrEditCompanyAddressViewModel>();
        CreateMap<CompanyContactDto, CreateOrEditCompanyContactViewModel>();

        CreateMap<AccountingPeriodDto, EditAccountingPeriodViewModel>();
        CreateMap<CreateAccountingPeriodViewModel, AccountingPeriodCreateDto>();
        CreateMap<EditAccountingPeriodViewModel, AccountingPeriodUpdateDto>();

        CreateMap<AccountTypeDto, EditAccountTypeViewModel>();
        CreateMap<CreateAccountTypeViewModel, AccountTypeCreateDto>();
        CreateMap<EditAccountTypeViewModel, AccountTypeUpdateDto>();

        CreateMap<SubjectCategoryDto, EditSubjectCategoryViewModel>();
        CreateMap<CreateSubjectCategoryViewModel, SubjectCategoryCreateDto>();
        CreateMap<EditSubjectCategoryViewModel,SubjectCategoryUpdateDto>();

        CreateMap<SubjectDto, EditSubjectViewModel>();
        CreateMap<CreateSubjectViewModel, SubjectCreateDto>();
        CreateMap<EditSubjectViewModel, SubjectUpdateDto>();

        CreateMap<AccountingSettingDto, AccountingSettingViewModel>();
    }
}
