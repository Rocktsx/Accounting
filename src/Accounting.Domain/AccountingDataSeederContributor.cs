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
        private readonly IRepository<Company, Guid> _companyRepository;
        public AccountingDataSeederContributor(IRepository<Currency> currencyRepository, IRepository<Company, Guid> companyRepository)
        {
            _currencyRepository = currencyRepository;
            _companyRepository = companyRepository;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _currencyRepository.AnyAsync())
            {
                await _currencyRepository.InsertAsync(new Currency("RMB", "RMB", 1, 1, 1, DateOnly.FromDateTime(DateTime.Now), true)); 
                await _currencyRepository.InsertAsync(new Currency("RMB", "USD", 720, 100, 7.2m, DateOnly.FromDateTime(DateTime.Now), true)); 
            }
            if(!await _companyRepository.AnyAsync())
            { 
                var company = new Company(Guid.NewGuid(), "SUNKIST (FAR EAST) PROMOTION LTD.", "SUNKIST (FAR EAST) PROMOTION LTD.", "A-SUNKIST", "RMB", 0, "C.O.D.",string.Empty, true, false);
                company.SetCode("A-SUNKIST", "A-SUNKIST", 1);
                company.AddAddress(Guid.NewGuid(), true, false, "MANAGING DIRECTOR", "1303 BANK OF AMERICA TOWER12 HARCOURT ROADCENTRAL", "MARIA KWOK", "28453454", "SZ@SZ.COM", "SZ", "HK", "SZ", "SZ", "28453454");
                company.AddContact(Guid.NewGuid(), "MARIA KWOK", "SZ", "SZ", "0755-01254125", "0755-01254125", "0755-01254122", "", "SZ");
                await _companyRepository.InsertAsync(company); 
            }
        }
    }
}
