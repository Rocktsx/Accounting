using Accounting.BasicData;
using AutoMapper;

namespace Accounting;

public class AccountingApplicationAutoMapperProfile : Profile
{
    public AccountingApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Currency, CurrencyDto>();
        CreateMap<CurrencyCreateDato, Currency>();
    }
}
