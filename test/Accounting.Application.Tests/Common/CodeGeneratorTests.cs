using Accounting.BasicData;
using Accounting.Utility;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Common
{
    public abstract class CodeGeneratorTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly CodeGenerator _codeGenerator;

        public CodeGeneratorTests()
        {
            _companyRepository = GetRequiredService<ICompanyRepository>();
            _codeGenerator = GetRequiredService<CodeGenerator>();
        }
        [Fact]
        public async Task Can_Generate_Code()
        {
            // Arrange
            var company = new Company(Guid.NewGuid(), "Test Company", string.Empty,
                string.Empty, "USD", 0, string.Empty, string.Empty, true, false);
            company.SetPrefix("C");
            var company2 = new Company(Guid.NewGuid(), "Test2 Company", string.Empty,
               string.Empty, "USD", 0, string.Empty, string.Empty, true, false);
            company2.SetPrefix("C");
            var company3 = new Company(Guid.NewGuid(), "Test2 Company", string.Empty,
               string.Empty, "USD", 0, string.Empty, string.Empty, true, false);
            company3.SetPrefix("V");

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _codeGenerator.GenerateCodeAsync(company,
                _companyRepository, new CodeCacheItem
                {
                    FunctionCode = FunctionCodes.Client
                });
                await _codeGenerator.GenerateCodeAsync(company2,
               _companyRepository, new CodeCacheItem
               {
                   FunctionCode = FunctionCodes.Client
               });
                await _codeGenerator.GenerateCodeAsync(company3,
               _companyRepository, new CodeCacheItem
               {
                   FunctionCode = FunctionCodes.Client
               },()=> "VC", "{0:###000}");
            });

            // Assert
            company.Code.ShouldBe("C-0001");
            company.GenNo.ShouldBe(1);
            company2.Code.ShouldBe("C-0002");
            company2.GenNo.ShouldBe(2);
            company3.Code.ShouldBe("VC-001");
            company3.GenNo.ShouldBe(1);
        }
    }
}
