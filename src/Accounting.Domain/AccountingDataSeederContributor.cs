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
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Company, Guid> _companyRepository;
        private readonly IRepository<AccountingPeriod, Guid> _accountingPeriodRepository;
        private IGuidGenerator _guidGenerator;
        private readonly IRepository<AccountType, Guid> _accountTypeRepository;
        private List<AccountType> _accountTypes;
        private readonly IRepository<SubjectCategory, Guid> _subjectCategoryRepository;
        private readonly IRepository<Subject, Guid> _subjectRepository;
        public AccountingDataSeederContributor(IRepository<Currency> currencyRepository, IRepository<Company, Guid> companyRepository,
            IGuidGenerator guidGenerator, IRepository<AccountingPeriod, Guid> accountingPeriodRepository,
            IRepository<AccountType, Guid> accountTypeRepository, IRepository<SubjectCategory, Guid> subjectCategoryRepository, IRepository<Subject, Guid> subjectRepository)
        {
            _currencyRepository = currencyRepository;
            _companyRepository = companyRepository;
            _guidGenerator = guidGenerator;
            _accountingPeriodRepository = accountingPeriodRepository;
            _accountTypeRepository = accountTypeRepository;
            _subjectCategoryRepository = subjectCategoryRepository;
            _subjectRepository = subjectRepository;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _currencyRepository.AnyAsync())
            {
                await _currencyRepository.InsertAsync(new Currency(_guidGenerator.Create(), "RMB", "RMB", 1, 1, 1, DateOnly.FromDateTime(DateTime.Now), true));
                await _currencyRepository.InsertAsync(new Currency(_guidGenerator.Create(), "RMB", "USD", 720, 100, 7.2m, DateOnly.FromDateTime(DateTime.Now), true));
            }
            if (!await _companyRepository.AnyAsync())
            {
                var company = new Company(Guid.NewGuid(), "SUNKIST (FAR EAST) PROMOTION LTD.", "SUNKIST (FAR EAST) PROMOTION LTD.", "A-SUNKIST", "RMB", 0, "C.O.D.", string.Empty, true, false);
                company.SetCode("A-SUNKIST", "A-SUNKIST", 1);
                company.AddAddress(Guid.NewGuid(), true, false, "MANAGING DIRECTOR", "1303 BANK OF AMERICA TOWER12 HARCOURT ROADCENTRAL", "MARIA KWOK", "28453454", "SZ@SZ.COM", "SZ", "HK", "SZ", "SZ", "28453454");
                company.AddContact(Guid.NewGuid(), "MARIA KWOK", "SZ", "SZ", "0755-01254125", "0755-01254125", "0755-01254122", "", "SZ");
                await _companyRepository.InsertAsync(company);
            }
            if (!await _accountingPeriodRepository.AnyAsync())
            {
                var date = DateTime.Now;
                var accountingPeriod = new AccountingPeriod(Guid.NewGuid(), date.Year.ToString(), new DateOnly(date.Year, 1, 1), new DateOnly(date.Year, 12, 31), true);
                await _accountingPeriodRepository.InsertAsync(accountingPeriod);
            }
            await AddAccountType(context);
            if (!await _subjectCategoryRepository.AnyAsync() && _accountTypes != null)
            {
                var nonCurrentAccountType = _accountTypes.FirstOrDefault(a => a.Code == "NA");
                if (nonCurrentAccountType !=null)
                { 
                    var subjectCategory = new SubjectCategory(_guidGenerator.Create(), "1", "非流动资产", "Non-Current Assets", null, DebitorCreditor.Debitor, nonCurrentAccountType.Id, true, "Non-Current Assets");
                    await _subjectCategoryRepository.InsertAsync(subjectCategory); 
                }
            }
            if (!await _subjectRepository.AnyAsync() && _accountTypes != null)
            {
                var fixedAssetsAccountType = _accountTypes.FirstOrDefault(a => a.Code == "FA");
                if(fixedAssetsAccountType != null)
                { 
                    var subject = new Subject(_guidGenerator.Create(), "11", "固定资产", "Fixed Assets", null, fixedAssetsAccountType.Id, DebitorCreditor.Debitor, "RMB", "Fixed Assets", false, true, false, 0);
                    await _subjectRepository.InsertAsync(subject);
                }
            }
            _accountTypes = null;
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
            _accountTypes = new List<AccountType>
             {
                 new AccountType(assetsId,"A", "资产", "Assets", null, 1, 0, 1, 1, 0, 1),
                 new AccountType(nonCurrentAssetsId,"NA", "非流动资产", "Non-Current Assets", assetsId, 2, 0, 2, 1, 0, 1),
                 new AccountType(_guidGenerator.Create(),"FA", "固定资产", "Fixed Assets", nonCurrentAssetsId, 2, 0, 3, 1, 0, 1),
                 new AccountType(currentAssetsId,"CA", "流动资产", "Current Assets", assetsId, 3, 0, 4, 1, 0, 1),
                 new AccountType(_guidGenerator.Create(),"AR", "应收账", "Receivable", currentAssetsId, 6, 0, 6, 1, 0, 1),
                 new AccountType(_guidGenerator.Create(),"BAK", "银行", "Bank", currentAssetsId, 4, 0, 7, 1, 0, 1),
                 new AccountType(_guidGenerator.Create(),"CSH", "现金", "Cash", currentAssetsId, 5, 0, 5, 1, 0, 1),
                 new AccountType(capitalId,"C", "资本", "Capital", null, 12, 0, 8, 3, 0, 2),
                 new AccountType(_guidGenerator.Create(),"PL", "损益账", "Profit And Loss", capitalId, 14, 0, 16, 3, 0, 2),
                 new AccountType(_guidGenerator.Create(),"SH", "股本", "Share", capitalId, 13, 0, 15, 3, 0, 2),
                 new AccountType(expensesId,"E", "支出", "Expenses", null, 19, 0, 0, 5, 1, 0),
                 new AccountType(_guidGenerator.Create(),"DEX", "直接成本", "Direct Cost", expensesId, 20, 2, 0, 5, 1, 0),
                 new AccountType(_guidGenerator.Create(),"AEX", "行政费用", "Administration Cost", expensesId, 22, 6, 0, 5, 2, 0),
                 new AccountType(_guidGenerator.Create(), "FEX", "财务支出", "Financial Expense", expensesId, 21, 5, 0, 5, 2, 0),
                 new AccountType(_guidGenerator.Create(),"TEX", "稅務及股息支出", "Tax & Dividend Expenses", expensesId, 23, 7, 0, 5, 2, 0),
                 new AccountType(incomeId,"I", "收入", "Income", null, 15, 0, 0, 4, 1, 0),
                 new AccountType(_guidGenerator.Create(),"DIN", "业绩", "Revenue", incomeId, 16, 1, 0, 4, 1, 0),
                 new AccountType(_guidGenerator.Create(),"EIN", "非经常性收入", "Extra-ordinatory Income", incomeId, 18, 4, 0, 4, 2, 0),
                 new AccountType(_guidGenerator.Create(),"OIN", "其他收入", "Other Income", incomeId, 17, 3, 0, 4, 1, 0),
                 new AccountType(liabilitiesId,"L", "负债", "Liabilities", null, 8, 0, 11, 2, 0, 2),
                 new AccountType(currentLiabilitiesId,"CL", "流动负债", "Current Liabilities", liabilitiesId, 9, 0, 14, 2, 0, 2),
                 new AccountType(_guidGenerator.Create(),"AP", "应付账", "Payable", currentLiabilitiesId, 10, 0, 15, 2, 0, 2),
                 new AccountType(nonCurrentLiabilitiesId,"NL", "非流动负债", "Non-Current Liabilities", liabilitiesId,11, 0, 12, 2, 0, 2),
                 new AccountType(_guidGenerator.Create(),"LL", "长期负债", "Long Term Liabilities", nonCurrentLiabilitiesId, 11, 0, 13, 2, 0, 2)
             };
            await _accountTypeRepository.InsertManyAsync(_accountTypes);
        }
    }
}
