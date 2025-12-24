using Accounting.Finance.BankReconciliations;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class BankReconciliationAppServiceTests<TStartupModule> : 
        AccountingApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
    {
        private readonly IBankReconciliationAppService _bankReconciliationAppService;
        private readonly IBankReconciliationRepository _bankReconciliationRepository;
        private readonly AccountingTestData _testData;

        public BankReconciliationAppServiceTests()
        {
            _bankReconciliationAppService = GetRequiredService<IBankReconciliationAppService>();
            _bankReconciliationRepository = GetRequiredService<IBankReconciliationRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Create_Bank_Reconciliation()
        {
            // arrange
            var list = await _bankReconciliationRepository.GetPagedListAsync();
            var voucherDetailItem = list.Where(item => item.VoucherDetailId != _testData.VoucherDetailId).First(); 
            var dto = new BankReconciliationCreateDto
            {
                VoucherDetailId = voucherDetailItem.VoucherDetailId,
                IsPresented = true,
            };

            // act
            var newDto = await _bankReconciliationAppService.CreateAsync(dto);

            // assert
            var entity = await _bankReconciliationRepository.GetAsync(newDto.Id);
            entity.ShouldNotBeNull();
            entity.VoucherDetailId.ShouldBe(voucherDetailItem.VoucherDetailId);
            entity.IsPresented.ShouldBeTrue();
        }
        [Fact]
        public async Task Can_Get_Bank_Reconciliation()
        {
            // act
            var dto = await _bankReconciliationAppService.GetAsync(_testData.BankReconciliationId);

            // assert
            dto.ShouldNotBeNull();
            dto.VoucherDetailId.ShouldBe(_testData.VoucherDetailId);
            dto.IsPresented.ShouldBeTrue();
        }

        [Fact]
        public async Task Can_Delete_Bank_Reconciliation()
        {
            // act
            await _bankReconciliationAppService.DeleteAsync(_testData.BankReconciliationId);

            // assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException<BankReconciliation>>(async () =>
            {
                await _bankReconciliationAppService.GetAsync(_testData.BankReconciliationId);
            });
            exception.ShouldNotBeNull(); 
        }

        [Fact]
        public async Task Can_Get_List()
        {
            // arrange
            var input = new BankReconciliationPagedRequestDto
            {
                StartNo = 1,
                MaxResultCount = 10,
            };

            // act
            var result = await _bankReconciliationAppService.GetListAsync(input);

            // assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(7);
        }
    }
}
