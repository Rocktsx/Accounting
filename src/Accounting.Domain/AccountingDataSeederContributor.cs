using Accounting.BasicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Accounting
{
    public class AccountingDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Currency> _currencyRepository;
        public AccountingDataSeederContributor(IRepository<Currency> currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _currencyRepository.AnyAsync())
            {
                await _currencyRepository.InsertAsync(new Currency("RMB", "RMB", 1, 1, 1, DateOnly.FromDateTime(DateTime.Now), true)); 
                await _currencyRepository.InsertAsync(new Currency("RMB", "USD", 720, 100, 7.2m, DateOnly.FromDateTime(DateTime.Now), true)); 
            }
        }
    }
}
