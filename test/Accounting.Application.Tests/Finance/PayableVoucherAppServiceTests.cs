using Accounting.Finance.PayableVouchers;
using Accounting.Finance.Settings;
using Accounting.Finance.Vouchers;
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
    }
}
