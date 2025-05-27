using Accounting.BasicData;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Xunit;

namespace Accounting.Common
{
    public abstract class GenerateCodeServiceTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IRepository<Company, Guid> _companyRepository;
        private readonly GenerateCodeService _generateCodeService = new GenerateCodeService();
        protected GenerateCodeServiceTests()
        {
            _companyRepository = GetRequiredService<IRepository<Company, Guid>>();
        } 
        [Fact]
        public async Task Can_Set_Code()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange 
                var company = new Company(Guid.NewGuid(), "Test Company 2", null, null, "RMB", 0, null, null, true, false);
                company.SetCode("-", "T", 1);
                // Act
                await _generateCodeService.GenerateCodeAsync(company, _companyRepository);
                // Assert
                company.Code.ShouldBe("T-0001");
                company.GenNo.ShouldBe(1);
            });
        }

    }
}
