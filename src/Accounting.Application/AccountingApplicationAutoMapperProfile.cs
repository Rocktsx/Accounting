using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
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
        CreateMap<CompanyCreateDto, Company>();
        CreateMap<CompanyAddress, CompanyAddressDto>();
        CreateMap<CompanyAddressCreateDto, CompanyAddress>();
        CreateMap<CompanyContact, CompanyContactDto>();
        CreateMap<CompanyContactCreateDto, CompanyContact>();

        CreateMap<AccountingPeriod, AccountingPeriodDto>();
        CreateMap<AccountType, AccountTypeDto>();
        CreateMap<AccountType, AccountTypeSimpleDto>();
        CreateMap<SubjectCategory,SubjectCategoryDto>();
        CreateMap<SubjectCategory, SubjectCategoryFilteredResultDto>();
        CreateMap<Subject, SubjectDto>();

        CreateMap<Voucher, VoucherDto>();
        CreateMap<VoucherDetail, VoucherDetailDto>();
    }
}
