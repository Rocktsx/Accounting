using Accounting.BasicData;
using Accounting.BasicData.Dtos;
using Accounting.Finance.Dtos;
using Accounting.Web.Pages.BasicData.Currency;
using Accounting.Web.ViewModels;
using AutoMapper;

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
    }
}
