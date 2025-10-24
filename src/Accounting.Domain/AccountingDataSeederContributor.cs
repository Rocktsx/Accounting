using Accounting.BasicData;
using Accounting.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Accounting
{
    public class AccountingDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IAccountingPeriodRepository _accountingPeriodRepository;
        private IGuidGenerator _guidGenerator;
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly ISubjectCategoryRepository _subjectCategoryRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IVoucherRepository _voucherRepository;

        private Guid? _subject2801Id = Guid.Empty;
        private Guid? _subject8021Id = Guid.Empty;

        public AccountingDataSeederContributor(ICurrencyRepository currencyRepository,
            ICompanyRepository companyRepository,
            IGuidGenerator guidGenerator, IAccountingPeriodRepository accountingPeriodRepository,
            IAccountTypeRepository accountTypeRepository, ISubjectCategoryRepository subjectCategoryRepository,
            ISubjectRepository subjectRepository, IVoucherRepository voucherRepository)
        {
            _currencyRepository = currencyRepository;
            _companyRepository = companyRepository;
            _guidGenerator = guidGenerator;
            _accountingPeriodRepository = accountingPeriodRepository;
            _accountTypeRepository = accountTypeRepository;
            _subjectCategoryRepository = subjectCategoryRepository;
            _subjectRepository = subjectRepository;
            _voucherRepository = voucherRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _currencyRepository.AnyAsync())
            {
                await _currencyRepository.InsertAsync(new Currency(_guidGenerator.Create(), "RMB", "RMB", 1, 1, 1,
                    DateOnly.FromDateTime(DateTime.Now), true, context.TenantId));
                await _currencyRepository.InsertAsync(new Currency(_guidGenerator.Create(), "RMB", "USD", 720, 100,
                    7.2m, DateOnly.FromDateTime(DateTime.Now), true, context.TenantId));
            }

            if (!await _companyRepository.AnyAsync())
            {
                var company = new Company(Guid.NewGuid(), "SUNKIST (FAR EAST) PROMOTION LTD.",
                    "SUNKIST (FAR EAST) PROMOTION LTD.", "A-SUNKIST",
                    "RMB", 0, "C.O.D.", string.Empty, true, false, context.TenantId);
                company.SetCode("A-SUNKIST", "A-SUNKIST", 1);
                company.AddAddress(Guid.NewGuid(), true, false, "MANAGING DIRECTOR",
                    "1303 BANK OF AMERICA TOWER12 HARCOURT ROADCENTRAL", "MARIA KWOK",
                    "28453454", "SZ@SZ.COM", "SZ", "HK", "SZ", "SZ", "28453454");
                company.AddContact(Guid.NewGuid(), "MARIA KWOK", "SZ", "SZ", "0755-01254125", "0755-01254125",
                    "0755-01254122", "", "SZ");
                await _companyRepository.InsertAsync(company);
            }

            if (!await _accountingPeriodRepository.AnyAsync())
            {
                var year = 2025;
                var accountingPeriod = new AccountingPeriod(Guid.NewGuid(), year.ToString(), new DateOnly(year, 1, 1),
                    new DateOnly(year, 12, 31), true, context.TenantId);
                await _accountingPeriodRepository.InsertAsync(accountingPeriod);
            }

            await AddAccountType(context);
            if (!await _subjectCategoryRepository.AnyAsync())
            { 
                var nonCurrentAccountType = await _accountTypeRepository.FirstOrDefaultAsync(a => a.Code == "NA");

                var subjectCategory = new SubjectCategory(_guidGenerator.Create(), "1", "非流动资产", "Non-Current Assets",
                    null, DebitorCreditor.Debitor,
                    nonCurrentAccountType?.Id, true, "Non-Current Assets", context.TenantId);
                await _subjectCategoryRepository.InsertAsync(subjectCategory, true);
            }

            if (!await _subjectRepository.AnyAsync())
            {
                await AddSubjectAsync(true, context.TenantId);
                await AddSubjectAsync(false, context.TenantId);
            }

            if (!await _voucherRepository.AnyAsync())
            {
                var voucher = new Voucher(_guidGenerator.Create(), new DateOnly(2025, 1, 1), VoucherType.JournalVoucher,
                    VoucherStatus.Draft, context.TenantId);
                voucher.SetCode("JV-0001", "JV", 1);
                if (_subject2801Id == null || _subject2801Id.Value.Equals(Guid.Empty))
                {
                    await AddSubjectAsync(true, context.TenantId);
                }

                if (_subject8021Id == null || _subject8021Id.Value.Equals(Guid.Empty))
                {
                    await AddSubjectAsync(false, context.TenantId);
                }

                voucher.AddDetail(_guidGenerator.Create(), _subject2801Id.Value, null, "Rent & Rates 2011 01",
                    DebitorCreditor.Debitor, "RMB", 1, 12600.0000m, 12600.0000m, string.Empty, null, 0, false,
                    string.Empty);
                voucher.AddDetail(_guidGenerator.Create(), _subject8021Id.Value, null, "Rent & Rates 2011 01",
                    DebitorCreditor.Creditor, "RMB", 1, 12600.0000m, 12600.0000m, string.Empty, null, 0, false,
                    string.Empty);
                await _voucherRepository.InsertAsync(voucher);
            }
        }

        private async Task AddSubjectAsync(bool isAddBankOrAddRentRate, Guid? tenantId = null)
        {
            if (isAddBankOrAddRentRate)
            {
                var subject2801= await AddOrGetSubjectAsync("2801", "BAK", "銀行 (往來戶口）", "Bank (C/A)", tenantId);
                _subject2801Id = subject2801.Id;
                return;
            }

            var subject8021 = await AddOrGetSubjectAsync("8021", "AEX", "租金及差餉", "Rent & Rates", tenantId);
            _subject8021Id = subject8021.Id;
        }

        private async Task<Subject> AddOrGetSubjectAsync(string subjectCode, string accountTypeCode, string name,
            string otherName, Guid? tenantId = null)
        {
            var existsSubject = await _subjectRepository.FirstOrDefaultAsync(item => item.Code == subjectCode);
            if (existsSubject != null)
            { 
                return existsSubject;
            }

            var accountType = await _accountTypeRepository.FirstOrDefaultAsync(a => a.Code == accountTypeCode); 
            var subject = new Subject(_guidGenerator.Create(), subjectCode, name, otherName, null, accountType?.Id,
                DebitorCreditor.Debitor, "RMB", name, false, true, true, 0, tenantId);
            await _subjectRepository.InsertAsync(subject, true);
            return subject;
        }

        private async Task AddAccountType(DataSeedContext context)
        {
            if (await _accountTypeRepository.AnyAsync())
            {
                return;
            }

            var assetsId = _guidGenerator.Create();
            var expensesId = _guidGenerator.Create();
            var capitalId = _guidGenerator.Create();
            var currentAssetsId = _guidGenerator.Create();
            var currentLiabilitiesId = _guidGenerator.Create();
            var incomeId = _guidGenerator.Create();
            var nonCurrentAssetsId = _guidGenerator.Create();
            var liabilitiesId = _guidGenerator.Create();
            var nonCurrentLiabilitiesId = _guidGenerator.Create();
            var accountTypes = new List<AccountType>
            {
                new AccountType(assetsId, "A", "资产", "Assets", null, 1, 0, 1, 1, 0, 1, context.TenantId),
                new AccountType(nonCurrentAssetsId, "NA", "非流动资产", "Non-Current Assets", assetsId, 2, 0, 2, 1, 0, 1,
                    context.TenantId),
                new AccountType(_guidGenerator.Create(), "FA", "固定资产", "Fixed Assets", nonCurrentAssetsId, 2, 0, 3, 1,
                    0, 1, context.TenantId),
                new AccountType(currentAssetsId, "CA", "流动资产", "Current Assets", assetsId, 3, 0, 4, 1, 0, 1,
                    context.TenantId),
                new AccountType(_guidGenerator.Create(), "AR", "应收账", "Receivable", currentAssetsId, 6, 0, 6, 1, 0, 1,
                    context.TenantId, AccountTypeTypes.Receivable),
                new AccountType(_guidGenerator.Create(), "BAK", "银行", "Bank", currentAssetsId, 4, 0, 7, 1, 0, 1,
                    context.TenantId, AccountTypeTypes.Bank),
                new AccountType(_guidGenerator.Create(), "CSH", "现金", "Cash", currentAssetsId, 5, 0, 5, 1, 0, 1,
                    context.TenantId),
                new AccountType(capitalId, "C", "资本", "Capital", null, 12, 0, 8, 3, 0, 2, context.TenantId),
                new AccountType(_guidGenerator.Create(), "PL", "损益账", "Profit And Loss", capitalId, 14, 0, 16, 3, 0, 2,
                    context.TenantId),
                new AccountType(_guidGenerator.Create(), "SH", "股本", "Share", capitalId, 13, 0, 15, 3, 0, 2,
                    context.TenantId),
                new AccountType(expensesId, "E", "支出", "Expenses", null, 19, 0, 0, 5, 1, 0, context.TenantId),
                new AccountType(_guidGenerator.Create(), "DEX", "直接成本", "Direct Cost", expensesId, 20, 2, 0, 5, 1, 0,
                    context.TenantId),
                new AccountType(_guidGenerator.Create(), "AEX", "行政费用", "Administration Cost", expensesId, 22, 6, 0, 5,
                    2, 0, context.TenantId),
                new AccountType(_guidGenerator.Create(), "FEX", "财务支出", "Financial Expense", expensesId, 21, 5, 0, 5, 2,
                    0, context.TenantId),
                new AccountType(_guidGenerator.Create(), "TEX", "稅務及股息支出", "Tax & Dividend Expenses", expensesId, 23, 7,
                    0, 5, 2, 0, context.TenantId),
                new AccountType(incomeId, "I", "收入", "Income", null, 15, 0, 0, 4, 1, 0, context.TenantId),
                new AccountType(_guidGenerator.Create(), "DIN", "业绩", "Revenue", incomeId, 16, 1, 0, 4, 1, 0,
                    context.TenantId),
                new AccountType(_guidGenerator.Create(), "EIN", "非经常性收入", "Extra-ordinatory Income", incomeId, 18, 4, 0,
                    4, 2, 0, context.TenantId),
                new AccountType(_guidGenerator.Create(), "OIN", "其他收入", "Other Income", incomeId, 17, 3, 0, 4, 1, 0,
                    context.TenantId),
                new AccountType(liabilitiesId, "L", "负债", "Liabilities", null, 8, 0, 11, 2, 0, 2, context.TenantId),
                new AccountType(currentLiabilitiesId, "CL", "流动负债", "Current Liabilities", liabilitiesId, 9, 0, 14, 2,
                    0, 2, context.TenantId),
                new AccountType(_guidGenerator.Create(), "AP", "应付账", "Payable", currentLiabilitiesId, 10, 0, 15, 2, 0,
                    2, context.TenantId, AccountTypeTypes.Payable),
                new AccountType(nonCurrentLiabilitiesId, "NL", "非流动负债", "Non-Current Liabilities", liabilitiesId, 11, 0,
                    12, 2, 0, 2, context.TenantId),
                new AccountType(_guidGenerator.Create(), "LL", "长期负债", "Long Term Liabilities", nonCurrentLiabilitiesId,
                    11, 0, 13, 2, 0, 2, context.TenantId)
            };
            await _accountTypeRepository.InsertManyAsync(accountTypes, true);
        }
    }
}