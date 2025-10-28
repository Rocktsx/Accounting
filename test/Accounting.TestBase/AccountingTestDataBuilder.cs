using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;
using Accounting.Finance;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Polly;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Accounting;

public class AccountingTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IAccountingPeriodRepository _accountingPeriodRepository;
    private readonly IAccountTypeRepository _accountTypeRepository;
    private IGuidGenerator _guidGenerator;
    private readonly ISubjectCategoryRepository _subjectCategoryRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly AccountingTestData _testData;
    public AccountingTestDataSeedContributor(ICurrentTenant currentTenant,
        ICurrencyRepository currencyRepository,
            ICompanyRepository companyRepository,
            IGuidGenerator guidGenerator,
            IAccountingPeriodRepository accountingPeriodRepository,
            IAccountTypeRepository accountTypeRepository,
            ISubjectCategoryRepository subjectCategoryRepository,
            ISubjectRepository subjectRepository,
            IVoucherRepository voucherRepository,
            AccountingTestData testData)
    {
        _currentTenant = currentTenant;
        _currencyRepository = currencyRepository;
        _companyRepository = companyRepository;
        _guidGenerator = guidGenerator;
        _accountingPeriodRepository = accountingPeriodRepository;
        _accountTypeRepository = accountTypeRepository;
        _subjectCategoryRepository = subjectCategoryRepository;
        _subjectRepository = subjectRepository;
        _voucherRepository = voucherRepository;
        _testData = testData;
    }
    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        using (_currentTenant.Change(context?.TenantId))
        {
            await SeedCurrencyDataAsync(context);
            await SeedCompanyDataAsync(context);
            await SeedAccountingPeriodDataAsync(context);
            await SeedSubjectCategoryDataAsync(context);
            await SeedSubjectAsync(context);
            await SeedVoucherDataAsync(context);
        }
    }

    private async Task SeedSubjectCategoryDataAsync(DataSeedContext context)
    {
        if (!await _subjectCategoryRepository.AnyAsync())
        {
            var nonCurrentAccountType = await _accountTypeRepository.FirstOrDefaultAsync(a => a.Code == _testData.AccountTypeNa);

            var subjectCategory = new SubjectCategory(_testData.SubjectCategoryId,
                _testData.SubjectCategoryCode, _testData.SubjectCategoryName,
                _testData.SubjectCategoryOtherName, null,
                DebitorCreditor.Debitor, nonCurrentAccountType?.Id, true,
                string.Empty, context.TenantId);

            await _subjectCategoryRepository.InsertAsync(subjectCategory, true);
        }
    }
    private async Task SeedAccountingPeriodDataAsync(DataSeedContext context)
    {
        if (!await _accountingPeriodRepository.AnyAsync())
        {
            var year = _testData.AccountingPeriodYear;
            var accountingPeriod = new AccountingPeriod(Guid.NewGuid(),
                year.ToString(), _testData.AccountingPeriodStartDate,
               _testData.AccountingPeriodEndDate, true, context.TenantId);
            await _accountingPeriodRepository.InsertAsync(accountingPeriod);
        }
    }
    private async Task SeedCurrencyDataAsync(DataSeedContext context)
    {
        if (!await _currencyRepository.AnyAsync())
        {
            await _currencyRepository.InsertManyAsync([
                new Currency(_guidGenerator.Create(), _testData.RmbCurrency,
                    _testData.RmbCurrency, 1, 1, 1,
                    DateOnly.FromDateTime(DateTime.Now), true, context.TenantId),
                new Currency(_guidGenerator.Create(), _testData.RmbCurrency,
                    _testData.UsdCurrency, 720, 100, 7.2m,
                    DateOnly.FromDateTime(DateTime.Now), true, context.TenantId)
            ]);
        }
    }
    private async Task SeedCompanyDataAsync(DataSeedContext context)
    {
        if (!await _companyRepository.AnyAsync())
        {
            var company = new Company(_testData.ClientId, _testData.ClientName,
                _testData.ClientOtherName, _testData.ClientNickName,
                _testData.RmbCurrency, 0, string.Empty, string.Empty, true,
                false, context.TenantId);

            company.SetCode(_testData.ClientCode, _testData.ClientCode, 1);

            company.AddAddress(Guid.NewGuid(), true, false,
                _testData.ClientAddressName, _testData.ClientAddress,
                _testData.ClientAddressContactPerson,
                _testData.ClientAddressTelephone, _testData.ClientAddressEmail,
                _testData.ClientAddressCountry, _testData.ClientAddressRegion,
                string.Empty, string.Empty, _testData.ClientAddressFax);

            company.AddContact(Guid.NewGuid(), _testData.ClientAddressContactPerson,
                _testData.ClientContactDeparment, _testData.ClientContactPosition,
                _testData.ClientAddressTelephone, _testData.ClientAddressTelephone,
                _testData.ClientAddressFax, _testData.ClientAddressEmail, string.Empty);

            var company2 = new Company(_testData.VendorId, _testData.VendorName,
                _testData.VendorOtherName, _testData.VendorNickName,
                _testData.RmbCurrency, 0, string.Empty, string.Empty, false,
                true, context.TenantId);
            company2.SetCode(_testData.VendorCode, _testData.VendorCode, 1);

            await _companyRepository.InsertManyAsync([company, company2]);
        }
    }

    private async Task SeedSubjectAsync(DataSeedContext context)
    {
        if (await _subjectRepository.AnyAsync())
        {
            return;
        }
        var tenantId = context?.TenantId;

        await AddSubjectAsync(_testData.Subject2801Id, _testData.Subject2801Code,
            _testData.AccountTypeBank, _testData.Subject2801Name,
            _testData.Subject2801OtherName, tenantId);

        await AddSubjectAsync(_testData.Subject8021Id, _testData.Subject8021Code,
            _testData.AccountTypeAex, _testData.Subject8021Name,
            _testData.Subject8021OtherName, tenantId);

        await AddSubjectAsync(_testData.Subject25Id, _testData.Subject25Code,
          _testData.AccountTypeAr, _testData.Subject25Name,
          _testData.Subject25OtherName, tenantId, true);

        await AddSubjectAsync(_testData.Subject42Id, _testData.Subject42Code,
         _testData.AccountTypeAp, _testData.Subject42Name,
         _testData.Subject42OtherName, tenantId, true);
    }
    private async Task AddSubjectAsync(Guid id, string subjectCode, string accountTypeCode, string name,
           string otherName, Guid? tenantId = null, bool isSubSubjectType = false)
    {
        var accountType = await _accountTypeRepository.FirstOrDefaultAsync(
                a => a.Code == accountTypeCode);
        var subject = new Subject(id, subjectCode, name,
            otherName, null, accountType?.Id, DebitorCreditor.Debitor,
            _testData.RmbCurrency, name, isSubSubjectType, true, true, 0, tenantId);
        await _subjectRepository.InsertAsync(subject, true);
    }
    private async Task SeedVoucherDataAsync(DataSeedContext context)
    {
        if (!await _voucherRepository.AnyAsync())
        {
            var voucher = new Voucher(_guidGenerator.Create(),
                new DateOnly(2025, 1, 1), VoucherType.JournalVoucher,
                VoucherStatus.Draft, context.TenantId);
            voucher.SetCode(_testData.VoucherCode, _testData.VoucherPrefx, 1);

            voucher.AddDetail(_guidGenerator.Create(), _testData.Subject2801Id,
                null, _testData.VoucherDescription, DebitorCreditor.Debitor,
                _testData.RmbCurrency, 1, 12600.0m, 12600.0m, string.Empty,
                null, 0, false, string.Empty);
            voucher.AddDetail(_guidGenerator.Create(), _testData.Subject8021Id,
                null, _testData.VoucherDescription, DebitorCreditor.Creditor,
                _testData.RmbCurrency, 1, 12600.0m, 12600.0m, string.Empty,
                null, 0, false, string.Empty);
            await _voucherRepository.InsertAsync(voucher);
        }
    }
}
