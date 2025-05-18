using Accounting.BasicData;
using Accounting.Web.Pages.BasicData.Currency;
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
    }
}
