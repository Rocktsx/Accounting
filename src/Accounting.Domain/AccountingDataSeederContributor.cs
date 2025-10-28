using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;
using Accounting.Finance;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
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
        private IGuidGenerator _guidGenerator;
        private readonly IAccountTypeRepository _accountTypeRepository;

        public AccountingDataSeederContributor(IGuidGenerator guidGenerator,
            IAccountTypeRepository accountTypeRepository)
        {
            _guidGenerator = guidGenerator; 
            _accountTypeRepository = accountTypeRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await AddAccountTypesAsync(context);
        }
        private async Task AddAccountTypesAsync(DataSeedContext context)
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