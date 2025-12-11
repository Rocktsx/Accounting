using Accounting.Finance.Vouchers;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class VoucherRepositoryTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IVoucherRepository _voucherPepository;
        private readonly AccountingTestData _testData;

        public VoucherRepositoryTests()
        {
            _voucherPepository = GetRequiredService<IVoucherRepository>();
            _testData = GetRequiredService<AccountingTestData>();
        }

        [Fact]
        public async Task Can_Get_Vouchers()
        {
            // arrange 
            var request = new VoucherFilterRequest
            {
                Filter = _testData.VoucherCode,
                StartDate = new DateOnly(_testData.AccountingPeriodYear, 1, 1),
                EndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31),
                Prefix = _testData.VoucherPrefix,
                StartNo = 0,
                EndNo = 100,
                VoucherType = VoucherType.JournalVoucher,
                Status = VoucherStatus.Draft,
                Codes = [_testData.VoucherCode]
            };

            // act
            var result = await _voucherPepository.GetPagedListAsync(request);

            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
        }
        [Fact]
        public async Task Can_Get_Count()
        {
            // arrange 
            var request = new VoucherFilterRequest
            {
                Filter = _testData.VoucherCode,
                StartDate = new DateOnly(_testData.AccountingPeriodYear, 1, 1),
                EndDate = new DateOnly(_testData.AccountingPeriodYear, 12, 31),
                Prefix = _testData.VoucherPrefix,
                StartNo = 0,
                EndNo = 100,
                VoucherType = VoucherType.JournalVoucher,
                Status = VoucherStatus.Draft,
                Codes = [_testData.VoucherCode]
            };

            // act
            var result = await _voucherPepository.GetCountAsync(request);

            result.ShouldBe(1);
        }

        [Fact]
        public async Task Can_Get_Last_Number()
        {
            // arrange 
            var prefix = _testData.VoucherPrefix;

            // act
            var lastNumber = await _voucherPepository.GetLastNumberAsync(prefix);

            //
            lastNumber.ShouldBe(_testData.InsertedJournalVouchers);
        }

        [Fact]
        public async Task Can_Get_Payable_Details()
        {
            // Arrange
            var debitorId = _testData.VendorId;

            // Act
            var result = await _voucherPepository.GetPayableDetailsAsync(debitorId, maxResultCount: 10);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);

            var docNo1Item = result.First(item => item.DocNo == _testData.DocNo3);
            docNo1Item.PaidAmount.ShouldBe(0);
            docNo1Item.PaidNativeAmount.ShouldBe(0);
            docNo1Item.CurrentPaid.ShouldBe(0);
            docNo1Item.NativeCurrentPaid.ShouldBe(0);
            docNo1Item.OsAmount.ShouldBe(_testData.DocNo1NativeAmount);

            var docNo2Item = result.First(item => item.DocNo == _testData.DocNo4);
            var docNo2OsAmount = _testData.DocNo2Amount - _testData.DocNo2PaidAmount;
            docNo2Item.PaidAmount.ShouldBe(_testData.DocNo2PaidAmount);
            docNo2Item.PaidNativeAmount.ShouldBe(_testData.DocNo2PaidNativeAmount);
            docNo2Item.CurrentPaid.ShouldBe(0);
            docNo2Item.NativeCurrentPaid.ShouldBe(0);
            docNo2Item.OsAmount.ShouldBe(docNo2OsAmount);
        }
        [Fact]
        public async Task Can_Get_Payable_Details_Count()
        {
            // Arrange
            var debitorId = _testData.VendorId;

            // Act
            var result = await _voucherPepository.GetPayableDetailsCountAsync(debitorId);

            // Assert
            result.ShouldBe(2);
        }
        [Fact]
        public async Task Can_Get_Payable_Details_By_Id()
        {
            // Arrange
            var id = _testData.VoucherPvId;

            // Act
            var result = await _voucherPepository.GetPayableDetailsAsync(id);

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
        public async Task Can_Get_Receivable_Details()
        {
            // Arrange
            var creditorId = _testData.ClientId;

            // Act
            var result = await _voucherPepository.GetReceivableDetailsAsync(creditorId, maxResultCount: 10);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);

            var docNo1Item = result.First(item => item.DocNo == _testData.DocNo1);
            docNo1Item.PaidAmount.ShouldBe(0);
            docNo1Item.PaidNativeAmount.ShouldBe(0);
            docNo1Item.CurrentPaid.ShouldBe(0);
            docNo1Item.NativeCurrentPaid.ShouldBe(0);
            docNo1Item.OsAmount.ShouldBe(_testData.DocNo1NativeAmount);

            var docNo2Item = result.First(item => item.DocNo == _testData.DocNo2);
            var docNo2OsAmount = _testData.DocNo2Amount - _testData.DocNo2PaidAmount;
            docNo2Item.PaidAmount.ShouldBe(_testData.DocNo2PaidAmount);
            docNo2Item.PaidNativeAmount.ShouldBe(_testData.DocNo2PaidNativeAmount);
            docNo2Item.CurrentPaid.ShouldBe(_testData.DocNo2PaidAmount);
            docNo2Item.NativeCurrentPaid.ShouldBe(_testData.DocNo2PaidNativeAmount);
            docNo2Item.OsAmount.ShouldBe(docNo2OsAmount);
        }
        [Fact]
        public async Task Can_Get_Receivable_Details_Count()
        {
            // Arrange
            var creditorId = _testData.ClientId;

            // Act
            var result = await _voucherPepository.GetReceivableDetailsCountAsync(creditorId);

            // Assert
            result.ShouldBe(2);
        }
        [Fact]
        public async Task Can_Get_Receivable_Details_By_Id()
        {
            // Arrange
            var id = _testData.VoucherRvId;

            // Act
            var result = await _voucherPepository.GetReceivableDetailsAsync(id);

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
    }
}
