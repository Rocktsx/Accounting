using Accounting.BasicData;
using Accounting.Finance;
using Accounting.Finance.Dtos;
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
        CreateMap<CurrencyCreateDto, Currency>();

        CreateMap<Company, CompanyDto>();
        CreateMap<CompanyCreateOrEditDto, Company>();
        CreateMap<CompanyAddress, CompanyAddressDto>();
        CreateMap<CompanyAddressCreateOrEditDto, CompanyAddress>();
        CreateMap<CompanyContact, CompanyContactDto>();
        CreateMap<CompanyContactCreateOrEditDto, CompanyContact>();

        CreateMap<AccountingPeriod, AccountingPeriodDto>();
        CreateMap<AccountType, AccountTypeDto>();
    }
}
