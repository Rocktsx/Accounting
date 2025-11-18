using Accounting.Finance.ReceivableVouchers;
using Accounting.Finance.Settings;
using Accounting.Finance.Vouchers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class ReceivableVoucherAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IReceivableVoucherAppService _service;
        private readonly AccountingTestData _testData;
        public ReceivableVoucherAppServiceTests()
        {
            _service = GetRequiredService<IReceivableVoucherAppService>();
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
        private ReceivableDetailDto GetReceivableItem()
        {
            return new ReceivableDetailDto()
            {
                SubjectId = _testData.SubjectArId,
                CurrencyCode = _testData.UsdCurrency,
                CurrencyRate = _testData.UsdCurrencyRate,
                CurrentPaid = _testData.DocNo2PaidAmount,
                NativeCurrentPaid = _testData.UsdCurrencyRate *
                    _testData.DocNo2PaidAmount,
                DebitorCreditor = DebitorCreditor.Debitor,
                DocNo = _testData.DocNo2
            };
        }
        private async Task InitArSubject()
        {
            var appSettingService = GetRequiredService<IAccountingSettingAppService>();
            await appSettingService.UpdateAsync(new AccountingSettingDto
            {
                AccountReceivableSubjectCode = _testData.SubjectArId.ToString(),
            });
        }
        [Fact]
        public async Task Can_Get_Receivable_Details()
        {
            // Arrange
            var dto = new ReceivableDetailsByDebitorRequestDto
            {
                DebitorId = _testData.ClientId,
                MaxResultCount = 10
            };

            // Act
            var result = await _service.GetReceivableDetailsByDebitorAsync(dto);

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);

            var docNo1Item = result.Items.First(item => item.DocNo == _testData.DocNo1);
            docNo1Item.PaidAmount.ShouldBe(0);
            docNo1Item.PaidNativeAmount.ShouldBe(0);
            docNo1Item.CurrentPaid.ShouldBe(0);
            docNo1Item.NativeCurrentPaid.ShouldBe(0);
            docNo1Item.OsAmount.ShouldBe(_testData.DocNo1NativeAmount);

            var docNo2Item = result.Items.First(item => item.DocNo == _testData.DocNo2);
            var docNo2OsAmount = _testData.DocNo2Amount - _testData.DocNo2PaidAmount;
            docNo2Item.PaidAmount.ShouldBe(_testData.DocNo2PaidAmount);
            docNo2Item.PaidNativeAmount.ShouldBe(_testData.DocNo2PaidNativeAmount);
            docNo2Item.CurrentPaid.ShouldBe(_testData.DocNo2PaidAmount);
            docNo2Item.NativeCurrentPaid.ShouldBe(_testData.DocNo2PaidNativeAmount);
            docNo2Item.OsAmount.ShouldBe(docNo2OsAmount);
        }
        [Fact]
        public async Task Can_Get_Receivable_Details_By_Id()
        {
            // Arrange
            var id = _testData.VoucherRvId;

            // Act
            var result = await _service.GetReceivableDetailsAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);

            var item = result.First();
            var osAmount = _testData.DocNo2Amount - _testData.DocNo2PaidAmount;
            item.PaidAmount.ShouldBe(_testData.DocNo2PaidAmount);
            item.PaidNativeAmount.ShouldBe(_testData.DocNo2PaidNativeAmount);
            item.OsAmount.ShouldBe(osAmount);
            item.CurrentPaid.ShouldBe(_testData.DocNo2PaidAmount);
            item.NativeCurrentPaid.ShouldBe(_testData.DocNo2PaidNativeAmount);
        }
        [Fact]
        public async Task Cannot_Generate_Details_With_Empty_Creditor()
        {
            // Arrange
            var input = new GenerateReceivableDetailRequestDto();

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
            var input = new GenerateReceivableDetailRequestDto()
            {
                Creditor = _testData.ClientId,
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
            var payment = GetPaymentItem() ;
            var receipt = GetReceivableItem();
            var input = new GenerateReceivableDetailRequestDto()
            {
                Creditor = _testData.ClientId,
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
                && item.DebitorCreditor == payment.DebitorCreditor
            );
            result.ShouldContain(item =>
               item.SubjectId == receipt.SubjectId
               && item.CurrencyCode == receipt.CurrencyCode
               && item.CurrencyRate == receipt.CurrencyRate
               && item.ForeignAmount == receipt.CurrentPaid
               && item.NativeAmount == receipt.NativeCurrentPaid
               && item.DebitorCreditor == DebitorCreditor.Creditor
               && item.DocNo == receipt.DocNo
           );
        }
        [Fact]
        public async Task Can_Generate_Details_With_No_Receipt()
        {
            // Arrange
            var payment = GetPaymentItem(paymentReference : _testData.PaymentReference);
            await InitArSubject();
            var input = new GenerateReceivableDetailRequestDto()
            {
                Creditor = _testData.ClientId,
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
                && item.DebitorCreditor == payment.DebitorCreditor
            );
            result.ShouldContain(item =>
               item.SubjectId == _testData.SubjectArId
               && item.CurrencyCode == payment.CurrencyCode
               && item.CurrencyRate == payment.CurrencyRate
               && item.ForeignAmount == payment.ForeignAmount
               && item.NativeAmount == payment.NativeAmount
               && item.DebitorCreditor == DebitorCreditor.Creditor
               && item.DocNo == payment.PaymentReference
           );
        }
        [Fact]
        public async Task Can_Generate_Details_With_Deposit()
        {
            // Arrange
            var payment = GetPaymentItem(2, _testData.PaymentReference);
            var receipt = GetReceivableItem();
            await InitArSubject();
            var input = new GenerateReceivableDetailRequestDto()
            {
                Creditor = _testData.ClientId,
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
                && item.DebitorCreditor == payment.DebitorCreditor
            );
            result.ShouldContain(item =>
               item.SubjectId == receipt.SubjectId
               && item.CurrencyCode == receipt.CurrencyCode
               && item.CurrencyRate == receipt.CurrencyRate
               && item.ForeignAmount == receipt.CurrentPaid
               && item.NativeAmount == receipt.NativeCurrentPaid
               && item.DebitorCreditor == DebitorCreditor.Creditor
               && item.DocNo == receipt.DocNo
           );
            result.ShouldContain(item =>
               item.SubjectId == _testData.SubjectArId
               && item.CurrencyCode == payment.CurrencyCode
               && item.CurrencyRate == payment.CurrencyRate
               && item.ForeignAmount == receipt.CurrentPaid
               && item.NativeAmount == receipt.NativeCurrentPaid
               && item.DebitorCreditor == DebitorCreditor.Creditor
               && item.DocNo == payment.PaymentReference
           );
        }
        [Fact]
        public async Task Cannot_Generate_Details_With_No_Payment_Reference()
        {
            // Arrange 
            await InitArSubject();
            var input = new GenerateReceivableDetailRequestDto()
            {
                Creditor = _testData.ClientId,
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
        public async Task Cannot_Generate_Details_With_No_Set_Ar_Subject()
        {
            // Arrange  
            var input = new GenerateReceivableDetailRequestDto()
            {
                Creditor = _testData.ClientId,
                Payments = [GetPaymentItem(paymentReference: _testData.PaymentReference)],
                Receipts = []
            };

            // Act
            var result = await Should.ThrowAsync<BusinessException>(async () =>
                await _service.GenerateDetailsAsync(input));

            // Assert
            result.ShouldNotBeNull();
            result.Code.ShouldBe(VoucherErrorCodes.PleaseEnterArSubjectInSetting);
        }
    }
}
