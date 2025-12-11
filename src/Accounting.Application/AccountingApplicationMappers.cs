using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.GeneralLedgerReports;
using Accounting.Finance.PayableVouchers;
using Accounting.Finance.ReceivableVouchers;
using Accounting.Finance.Reports;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Accounting
{
    
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)] 
    public partial class CurrencyToCurrencyDtoMapper : MapperBase<Currency, CurrencyDto>
    { 
        public override partial CurrencyDto Map(Currency source);
         
        public override partial void Map(Currency source, CurrencyDto destination);
    }
   
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class CompanyToCompanyDtoMapper : MapperBase<Company, CompanyDto>
    { 
        public override partial CompanyDto Map(Company source);

        public override partial void Map(Company source, CompanyDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class CompanyAddressToCompanyAddressDtoMapper : MapperBase<CompanyAddress, CompanyAddressDto>
    {
        public override partial CompanyAddressDto Map(CompanyAddress source);

        public override partial void Map(CompanyAddress source, CompanyAddressDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class CompanyContactToCompanyContactDtoMapper : MapperBase<CompanyContact, CompanyContactDto>
    {
        public override partial CompanyContactDto Map(CompanyContact source);

        public override partial void Map(CompanyContact source, CompanyContactDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class AccountingPeriodToAccountingPeriodDtoMapper : MapperBase<AccountingPeriod, AccountingPeriodDto>
    {
        public override partial AccountingPeriodDto Map(AccountingPeriod source);

        public override partial void Map(AccountingPeriod source, AccountingPeriodDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class AccountTypeToAccountTypeDtoMapper : MapperBase<AccountType, AccountTypeDto>
    {
        [MapperIgnoreTarget(nameof(AccountTypeDto.Parent))]
        public override partial AccountTypeDto Map(AccountType source);

        [MapperIgnoreTarget(nameof(AccountTypeDto.Parent))]
        public override partial void Map(AccountType source, AccountTypeDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class AccountTypeToAccountTypeSimpleDtoMapper : MapperBase<AccountType, AccountTypeSimpleDto>
    { 
        public override partial AccountTypeSimpleDto Map(AccountType source);
         
        public override partial void Map(AccountType source, AccountTypeSimpleDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SubjectCategoryToSubjectCategoryDtoMapper : MapperBase<SubjectCategory, SubjectCategoryDto>
    {
        public override partial SubjectCategoryDto Map(SubjectCategory source);

        public override partial void Map(SubjectCategory source, SubjectCategoryDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SubjectCategoryToSubjectCategoryFilteredResultDtoMapper : MapperBase<SubjectCategory, SubjectCategoryFilteredResultDto>
    {
        [MapperIgnoreTarget(nameof(SubjectCategoryFilteredResultDto.Parent))]
        [MapperIgnoreTarget(nameof(SubjectCategoryFilteredResultDto.AccountType))]
        public override partial SubjectCategoryFilteredResultDto Map(SubjectCategory source);

        [MapperIgnoreTarget(nameof(SubjectCategoryFilteredResultDto.Parent))]
        [MapperIgnoreTarget(nameof(SubjectCategoryFilteredResultDto.AccountType))]
        public override partial void Map(SubjectCategory source, SubjectCategoryFilteredResultDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SubjectToSubjectDtoMapper : MapperBase<Subject, SubjectDto>
    {
        public override partial SubjectDto Map(Subject source);

        public override partial void Map(Subject source, SubjectDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SubjectToSubjectFilterResultDtoMapper : MapperBase<Subject, SubjectFilterResultDto>
    { 
        [MapperIgnoreTarget(nameof(SubjectFilterResultDto.AccountType))]
        public override partial SubjectFilterResultDto Map(Subject source);

        [MapperIgnoreTarget(nameof(SubjectFilterResultDto.AccountType))]
        public override partial void Map(Subject source, SubjectFilterResultDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class VoucherToVoucherDtoMapper : MapperBase<Voucher, VoucherDto>
    {
        public override partial VoucherDto Map(Voucher source);

        public override partial void Map(Voucher source, VoucherDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class VoucherDetailToVoucherDetailDtoMapper : MapperBase<VoucherDetail, VoucherDetailDto>
    {
        public override partial VoucherDetailDto Map(VoucherDetail source);

        public override partial void Map(VoucherDetail source, VoucherDetailDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class VoucherFilterRequestDtoToVoucherFilterRequest: MapperBase<VoucherFilterRequestDto, VoucherFilterRequest>
    {
        [MapperIgnoreTarget(nameof(VoucherFilterRequest.Codes))]
        public override partial VoucherFilterRequest Map(VoucherFilterRequestDto source);

        [MapperIgnoreTarget(nameof(VoucherFilterRequest.Codes))]
        public override partial void Map(VoucherFilterRequestDto source, VoucherFilterRequest destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ReceivablePayableDetailToReceivableDetailDto : MapperBase<ReceivablePayableDetail, ReceivableDetailDto>
    {
        public override partial ReceivableDetailDto Map(ReceivablePayableDetail source);
        public override partial void Map(ReceivablePayableDetail source, ReceivableDetailDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ReceivablePayableDetailToPayableDetailDto : MapperBase<ReceivablePayableDetail, PayableDetailDto>
    {
        public override partial PayableDetailDto Map(ReceivablePayableDetail source);
        public override partial void Map(ReceivablePayableDetail source, PayableDetailDto destination);
    }
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class GLSingleCurrencyReportResultToGLSingleCurrencyReportResultDto : MapperBase<GeneralLedgerSingleCurrencyReportResult, GeneralLedgerSingleCurrencyReportResultDto>
    {
        public override partial GeneralLedgerSingleCurrencyReportResultDto Map(GeneralLedgerSingleCurrencyReportResult source);
        public override partial void Map(GeneralLedgerSingleCurrencyReportResult source, GeneralLedgerSingleCurrencyReportResultDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class GLMultipleCurrencyReportResultToGLMultipleCurrencyReportResultDto : MapperBase<GeneralLedgerMultipleCurrencyReportResult, GeneralLedgerMultipleCurrencyReportResultDto>
    {
        public override partial GeneralLedgerMultipleCurrencyReportResultDto Map(GeneralLedgerMultipleCurrencyReportResult source);
        public override partial void Map(GeneralLedgerMultipleCurrencyReportResult source, GeneralLedgerMultipleCurrencyReportResultDto destination);
    }
}
