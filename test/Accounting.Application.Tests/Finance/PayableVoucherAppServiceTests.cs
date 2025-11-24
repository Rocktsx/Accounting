using Accounting.Finance.PayableVouchers;
using Accounting.Finance.Settings;
using Accounting.Finance.Vouchers;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class PayableVoucherAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IPayableVoucherAppService _service;
        private readonly AccountingTestData _testData;
        public PayableVoucherAppServiceTests()
        {
            _service = GetRequiredService<IPayableVoucherAppService>();
            _testData = GetRequiredService<AccountingTestData>();
        }
        private PaymentDetailDto GetPaymentItem(int count = 1, string paymentReference = "")
        {
            return new PaymentDetailDto()
            {
                SubjectId = _testData.SubjectBankId,
                CurrencyCode = _testData.UsdCurrency,
                CurrencyRate = _testData.UsdCurrencyRate,
                ForeignAmount = count * _testData.DocNo2PaidAmount,
                DebitorCreditor = DebitorCreditor.Debitor,
                NativeAmount = _testData.UsdCurrencyRate * count *
                     _testData.DocNo2PaidAmount,
                PaymentReference = paymentReference
            };
        }
        private PayableDetailDto GetPayableItem()
        {
            return new PayableDetailDto()
            {
                SubjectId = _testData.SubjectApId,
                CurrencyCode = _testData.UsdCurrency,
                CurrencyRate = _testData.UsdCurrencyRate,
                CurrentPaid = _testData.DocNo2PaidAmount,
                NativeCurrentPaid = _testData.UsdCurrencyRate *
                    _testData.DocNo2PaidAmount,
                DebitorCreditor = DebitorCreditor.Creditor,
                DocNo = _testData.DocNo3
            };
        }
        private async Task InitArSubject()
        {
            var appSettingService = GetRequiredService<IAccountingSettingAppService>();
            await appSettingService.UpdateAsync(new AccountingSettingDto
            {
                AccountPayableSubjectCode = _testData.SubjectApId.ToString(),
            });
        }
        [Fact]
        public async Task Can_Get_Payable_Details()
        {
            // Arrange
            var dto = new PayableDetailByDebitorRequestDto
            {
                DebitorId = _testData.VendorId,
                MaxResultCount = 10
            };

            // Act
            var result = await _service.GetPayableDetailsByDebitorAsync(dto);

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);

            var docNo1Item = result.Items.First(item => item.DocNo == _testData.DocNo3);
            docNo1Item.PaidAmount.ShouldBe(0);
            docNo1Item.PaidNativeAmount.ShouldBe(0);
            docNo1Item.CurrentPaid.ShouldBe(0);
            docNo1Item.NativeCurrentPaid.ShouldBe(0);
            docNo1Item.OsAmount.ShouldBe(_testData.DocNo1NativeAmount);

            var docNo2Item = result.Items.First(item => item.DocNo == _testData.DocNo4);
            var docNo2OsAmount = _testData.DocNo2Amount - _testData.DocNo2PaidAmount;
            docNo2Item.PaidAmount.ShouldBe(_testData.DocNo2PaidAmount);
            docNo2Item.PaidNativeAmount.ShouldBe(_testData.DocNo2PaidNativeAmount);
            docNo2Item.CurrentPaid.ShouldBe(0);
            docNo2Item.NativeCurrentPaid.ShouldBe(0);
            docNo2Item.OsAmount.ShouldBe(docNo2OsAmount);
        }
        [Fact]
        public async Task Can_Get_Payable_Details_By_Id()
        {
            // Arrange
            var id = _testData.VoucherPvId;

            // Act
            var result = await _service.GetPayableDetailsAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);

            var item = result.First();
            var osAmount = _testData.DocNo2Amount - _testData.DocNo2PaidAmount;
            item.PaidAmount.ShouldBe(0);
            item.PaidNativeAmount.ShouldBe(0);
            item.OsAmount.ShouldBe(osAmount);
            item.CurrentPaid.ShouldBe(_testData.DocNo2PaidAmount);
            item.NativeCurrentPaid.ShouldBe(_testData.DocNo2PaidNativeAmount);
        }
        [Fact]
        public async Task Cannot_Generate_Details_With_Empty_Debitor()
        {
            // Arrange
            var input = new GeneratePayableDetailRequestDto();

            // Act
            var result = await Should.ThrowAsync<ArgumentException>(async () =>
                await _service.GenerateDetailsAsync(input));

            //Assert
            result.ShouldNotBeNull();
        }
        [Fact]
        public async Task Can_Generate_Empty_Details()
        {
            // Arrange
            var input = new GeneratePayableDetailRequestDto()
            {
                DebitorId = _testData.VendorId,
                Payments = [new() { NativeAmount = 0 }]
            };

            // Act
            var result = await _service.GenerateDetailsAsync(input);

            // Assert
            result.ShouldBeEmpty();
        }
        [Fact]
        public async Task Can_Generate_Details()
        {
            // Arrange
            var payment = GetPaymentItem();
            var receipt = GetPayableItem();
            var input = new GeneratePayableDetailRequestDto()
            {
                DebitorId = _testData.VendorId,
                Payments = [payment],
                Receipts = [receipt]
            };

            // Act
            var result = await _service.GenerateDetailsAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(item =>
                item.SubjectId == payment.SubjectId
                && item.CurrencyCode == payment.CurrencyCode
                && item.CurrencyRate == payment.CurrencyRate
                && item.ForeignAmount == payment.ForeignAmount
                && item.NativeAmount == payment.NativeAmount
                && item.DebitorCreditor == payment.DebitorCreditor.Reverse()
            );
            result.ShouldContain(item =>
               item.SubjectId == receipt.SubjectId
               && item.CurrencyCode == receipt.CurrencyCode
               && item.CurrencyRate == receipt.CurrencyRate
               && item.ForeignAmount == receipt.CurrentPaid
               && item.NativeAmount == receipt.NativeCurrentPaid
               && item.DebitorCreditor == DebitorCreditor.Debitor
               && item.DocNo == receipt.DocNo
           );
        }
        [Fact]
        public async Task Can_Generate_Details_With_No_Receipt()
        {
            // Arrange
            var payment = GetPaymentItem(paymentReference: _testData.PaymentReference);
            await InitArSubject();
            var input = new GeneratePayableDetailRequestDto()
            {
                DebitorId = _testData.VendorId,
                Payments = [payment],
                Receipts = []
            };

            // Act
            var result = await _service.GenerateDetailsAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(item =>
                item.SubjectId == payment.SubjectId
                && item.CurrencyCode == payment.CurrencyCode
                && item.CurrencyRate == payment.CurrencyRate
                && item.ForeignAmount == payment.ForeignAmount
                && item.NativeAmount == payment.NativeAmount
                && item.DebitorCreditor == payment.DebitorCreditor.Reverse()
            );
            result.ShouldContain(item =>
               item.SubjectId == _testData.SubjectApId
               && item.CurrencyCode == payment.CurrencyCode
               && item.CurrencyRate == payment.CurrencyRate
               && item.ForeignAmount == payment.ForeignAmount
               && item.NativeAmount == payment.NativeAmount
               && item.DebitorCreditor == DebitorCreditor.Debitor
               && item.DocNo == payment.PaymentReference
           );
        }
        [Fact]
        public async Task Can_Generate_Details_With_Deposit()
        {
            // Arrange
            var payment = GetPaymentItem(2, _testData.PaymentReference);
            var receipt = GetPayableItem();
            await InitArSubject();
            var input = new GeneratePayableDetailRequestDto()
            {
                DebitorId = _testData.VendorId,
                Payments = [payment],
                Receipts = [receipt]
            };

            // Act
            var result = await _service.GenerateDetailsAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            result.ShouldContain(item =>
                item.SubjectId == payment.SubjectId
                && item.CurrencyCode == payment.CurrencyCode
                && item.CurrencyRate == payment.CurrencyRate
                && item.ForeignAmount == payment.ForeignAmount
                && item.NativeAmount == payment.NativeAmount
                && item.DebitorCreditor == payment.DebitorCreditor.Reverse()
            );
            result.ShouldContain(item =>
               item.SubjectId == receipt.SubjectId
               && item.CurrencyCode == receipt.CurrencyCode
               && item.CurrencyRate == receipt.CurrencyRate
               && item.ForeignAmount == receipt.CurrentPaid
               && item.NativeAmount == receipt.NativeCurrentPaid
               && item.DebitorCreditor == DebitorCreditor.Debitor
               && item.DocNo == receipt.DocNo
           );
            result.ShouldContain(item =>
               item.SubjectId == _testData.SubjectApId
               && item.CurrencyCode == payment.CurrencyCode
               && item.CurrencyRate == payment.CurrencyRate
               && item.ForeignAmount == receipt.CurrentPaid
               && item.NativeAmount == receipt.NativeCurrentPaid
               && item.DebitorCreditor == DebitorCreditor.Debitor
               && item.DocNo == payment.PaymentReference
           );
        }
        [Fact]
        public async Task Cannot_Generate_Details_With_No_Payment_Reference()
        {
            // Arrange 
            await InitArSubject();
            var input = new GeneratePayableDetailRequestDto()
            {
                DebitorId = _testData.VendorId,
                Payments = [GetPaymentItem()],
                Receipts = []
            };

            // Act
            var result = await Should.ThrowAsync<BusinessException>(async () =>
                await _service.GenerateDetailsAsync(input));

            // Assert
            result.ShouldNotBeNull();
            result.Code.ShouldBe(VoucherErrorCodes.PaymentReferenceCannotBeEmpty);
        }
        [Fact]
        public async Task Cannot_Generate_Details_With_No_Set_Ap_Subject()
        {
            // Arrange  
            var input = new GeneratePayableDetailRequestDto()
            {
                DebitorId = _testData.VendorId,
                Payments = [GetPaymentItem(paymentReference: _testData.PaymentReference)],
                Receipts = []
            };

            // Act
            var result = await Should.ThrowAsync<BusinessException>(async () =>
                await _service.GenerateDetailsAsync(input));

            // Assert
            result.ShouldNotBeNull();
            result.Code.ShouldBe(VoucherErrorCodes.PleaseEnterApSubjectInSetting);
        }
    }
}
