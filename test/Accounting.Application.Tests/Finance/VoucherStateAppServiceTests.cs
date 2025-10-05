using Accounting.Finance.Settings;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Accounting.Finance.VoucherStates;
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
    public abstract class VoucherStateAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IVoucherAppService _voucherAppService;
        private readonly ISubjectAppService _subjectAppService;
        private const string BankSubjectCode = "2801";
        private const string RentAndRatesSubjectCode = "8021";
        private readonly IVoucherStateAppService _voucherStateAppService;

        public VoucherStateAppServiceTests()
        {
            _voucherAppService = GetRequiredService<IVoucherAppService>();
            _subjectAppService = GetRequiredService<ISubjectAppService>();
            _voucherStateAppService = GetRequiredService<IVoucherStateAppService>();
        }

        private async Task<VoucherDto> CreateVoucherAsync()
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
            var createdDto = await _voucherAppService.CreateAsync(dto);
            return createdDto;
        }
        [Fact]
        public async Task Can_Filter_Voucher()
        {
            // Arrange
            var dto = await CreateVoucherAsync();

            // Act
            var result = await _voucherStateAppService.GetListAsync(new VoucherUpdateStatusDto
            {
                VoucherType = dto.VoucherType,
                Code = dto.Code,
                Status = VoucherStatus.Draft
            });

            // Assert
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items.First().Id.ShouldBe(dto.Id); 
        }
    }
}
