using Accounting.Finance.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class TransferVoucherAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ITransferVoucherAppService _transferVoucherAppService;
        private readonly ISubjectAppService _subjectAppService;
        private const string BankSubjectCode = "2801";
        private const string RentAndRatesSubjectCode = "8021";

        public TransferVoucherAppServiceTests()
        {
            _transferVoucherAppService = GetRequiredService<ITransferVoucherAppService>();
            _subjectAppService = GetRequiredService<ISubjectAppService>();
        }
        private async Task<VoucherCreateDto> GetCreateDtoAsync()
        {
            var subjects = await _subjectAppService.GetSimpleListAsync();
            var dto = new VoucherCreateDto()
            {
                Prefix = "JV",
                VoucherType = VoucherType.JournalVoucher,
                VoucherDate = new DateTime(2025, 1, 12),
                Details =
                [
                    new VoucherDetailCreateDto()
                {
                    CurrencyCode = "RMB",
                    CurrencyRate = 1m,
                    ForeignAmount = 1000m,
                    NativeAmount = 1000m,
                    Description = "Rent & Rates 2025 01",
                    SubjectId = subjects.First(item => item.Code == BankSubjectCode).Id,
                    DebitorCreditor = DebitorCreditor.Creditor
                },
                new VoucherDetailCreateDto()
                {
                    CurrencyCode = "RMB",
                    CurrencyRate = 1m,
                    ForeignAmount = 1000m,
                    NativeAmount = 1000m,
                    Description = "Rent & Rates 2025 01",
                    SubjectId = subjects.First(item => item.Code == RentAndRatesSubjectCode).Id,
                    DebitorCreditor = DebitorCreditor.Debitor,
                    ItemQty = 0,
                    IsOriginal = false
                }
                ]
            };

            return dto;
        }

        [Fact]
        public async Task Can_Create_Voucher()
        {
            // Arrange
            var createDto = await GetCreateDtoAsync();
            createDto.VoucherType = VoucherType.PayableVoucher;

            // Act
            var dto = await _transferVoucherAppService.CreateAsync(createDto);

            // Assert
            dto.ShouldNotBeNull(); 
            dto.VoucherType.ShouldBe(VoucherType.JournalVoucher);
        }
        [Fact]
        public async Task Can_Get_Voucher_List_With_Voucher_Type()
        {
            // Arrange
            var createDto = await GetCreateDtoAsync();
            await _transferVoucherAppService.CreateAsync(createDto);

            // Act
            var result = await _transferVoucherAppService.GetListAsync(new VoucherFilterRequestDto()
            {
                MaxResultCount = 10,
                VoucherType = VoucherType.PayableVoucher
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
        }
    }
}
