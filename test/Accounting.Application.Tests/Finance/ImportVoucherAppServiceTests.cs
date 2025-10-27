namespace Accounting.Finance
{
    using Accounting.Finance.Subjects;
    using Accounting.Finance.Vouchers;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Volo.Abp;
    using Volo.Abp.Domain.Repositories;
    using Volo.Abp.Modularity;
    using Xunit;

    public abstract class ImportVoucherAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IImportVoucherAppService _importVoucherAppService;

        public ImportVoucherAppServiceTests()
        {
            _importVoucherAppService = GetRequiredService<IImportVoucherAppService>();
        }

        private VoucherImportItemDto GetVoucherImportItemDto()
        {
            return new VoucherImportItemDto
            {
                VoucherDate = new DateTime(2025, 1, 1),
                GroupNo = 1,
                VoucherCode = "V-001",
                Prefix = "V",
                SubjectCode = "2801",
                Debit = 100,
                Currency = "CNY",
                CurrencyRate = 1,
            };
        }

        [Fact]
        public async Task Can_Import_Double_Entry_Data()
        {
            // Arrange
            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.DoubleEntry,
                Data = new List<VoucherImportItemDto>
                {
                    GetVoucherImportItemDto(),
                    new VoucherImportItemDto
                    {
                        GroupNo = 1,
                        SubjectCode = "8021",
                        Credit = 100,
                        Currency = "CNY",
                        CurrencyRate = 1,
                    }
                }
            };

            // Act
            var result = await _importVoucherAppService.ImportDataAsync(input);

            // Assert
            result.ShouldBe(1);
        }

        [Fact]
        public async Task Can_Import_Single_Entry_Data()
        {
            // Arrange
            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.SingleEntry,
                SubjectCode = "8021",
                Data = new List<VoucherImportItemDto>
                {
                   GetVoucherImportItemDto(),
                     new VoucherImportItemDto
                    {
                        VoucherDate = new DateTime(2025, 1, 1),
                        GroupNo = 2,
                        VoucherCode = "V-002",
                        Prefix = "V",
                        SubjectCode = "2801",
                        Credit = 100,
                        Currency = "USD",
                        CurrencyRate = 7.2m,
                    }
                }
            };

            // Act
            var result = await _importVoucherAppService.ImportDataAsync(input);

            // Assert
            result.ShouldBe(2);
        }

        [Fact]
        public async Task Cannot_Import_Data_With_Empty_Subject_Code()
        {
            // Arrange
            var item = GetVoucherImportItemDto();
            item.SubjectCode = string.Empty;
            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.DoubleEntry,
                Data = new List<VoucherImportItemDto>
                {
                   item
                }
            };

            // Act
            var exception = await Should.ThrowAsync<BusinessException>(async () => await _importVoucherAppService.ImportDataAsync(input));

            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(VoucherErrorCodes.SubjectCodeCanNotBeEmpty);
        }

        [Fact]
        public async Task Cannot_Import_Data_With_Both_Debit_Credit()
        {
            // Arrange
            var item = GetVoucherImportItemDto();
            item.Credit = 100;
            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.DoubleEntry,
                Data = new List<VoucherImportItemDto>
                {
                   item
                }
            };

            // Act
            var exception = await Should.ThrowAsync<BusinessException>(async () => await _importVoucherAppService.ImportDataAsync(input));

            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(VoucherErrorCodes.DebitAndCreditCanNotBothBeGreaterThanZero);
        }

        [Fact]
        public async Task Cannot_Import_Data_With_Not_Exists_Subject_Code()
        {
            // Arrange
            var item = GetVoucherImportItemDto();
            item.SubjectCode = "11222dd";
            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.DoubleEntry,
                Data = new List<VoucherImportItemDto>
                {
                   item
                }
            };

            // Act
            var exception = await Should.ThrowAsync<BusinessException>(async () => await _importVoucherAppService.ImportDataAsync(input));

            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(VoucherErrorCodes.SubjectCodeNotExists);
        }

        [Fact]
        public async Task Cannot_Import_Data_With_Not_Exists_Sub_Subject_Code()
        {
            // Arrange
            var item = GetVoucherImportItemDto();
            item.SubSubjectCode = "11222dd";
            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.DoubleEntry,
                Data = new List<VoucherImportItemDto>
                {
                    item
                }
            };

            // Act
            var exception = await Should.ThrowAsync<BusinessException>(async () => await _importVoucherAppService.ImportDataAsync(input));

            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(VoucherErrorCodes.SubSubjectCodeNotExists);
        }

        [Fact]
        public async Task Cannot_Import_Single_Entry_Data_With_Empty_Subject_Code()
        {
            // Arrange
            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.SingleEntry,
                SubjectCode = string.Empty,
                Data = new List<VoucherImportItemDto>
                {
                    GetVoucherImportItemDto()
                }
            };

            // Act
            var exception = await Should.ThrowAsync<BusinessException>(async () => await _importVoucherAppService.ImportDataAsync(input));

            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(VoucherErrorCodes.InSingleEntrySubjectCodeCannotBeEmpty);
        }

        [Fact]
        public async Task Cannot_Import_Single_Entry_Data_With_Not_Exists_Subject_Code()
        {
            // Arrange
            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.SingleEntry,
                SubjectCode = "ddd22",
                Data = new List<VoucherImportItemDto>
                {
                   GetVoucherImportItemDto()
                }
            };

            // Act
            var exception = await Should.ThrowAsync<BusinessException>(async () => await _importVoucherAppService.ImportDataAsync(input));

            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(VoucherErrorCodes.SubjectCodeNotExists);
        }
        [Fact]
        public async Task Cannot_Import_Single_Entry_Data_With_Empty_Currency_Code()
        {
            // Arrange
            var subjectCode = "2801";
            await WithUnitOfWorkAsync(async () =>
             {
                 var subjectRepository = GetRequiredService<IRepository<Subject, Guid>>();
                 var subject = await subjectRepository.FirstOrDefaultAsync(s => s.Code == subjectCode);
                 subject.SetCurrencyCode(string.Empty);
                 await subjectRepository.UpdateAsync(subject);
             });

            var input = new VoucherImportDto
            {
                ImportType = VoucherImportType.SingleEntry,
                SubjectCode = subjectCode,
                Data = new List<VoucherImportItemDto>
                {
                   GetVoucherImportItemDto()
                }
            };
            // Act
            var exception = await Should.ThrowAsync<BusinessException>(async () => await _importVoucherAppService.ImportDataAsync(input));

            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(VoucherErrorCodes.CurrencyNotSetUpInSubject);
        }
    }
}
