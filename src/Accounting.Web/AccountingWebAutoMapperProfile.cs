using Accounting.BasicData;
using Accounting.Web.Pages.BasicData.Currency;
using AutoMapper;
using Accounting.Web.ViewModels;

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
    }
}
