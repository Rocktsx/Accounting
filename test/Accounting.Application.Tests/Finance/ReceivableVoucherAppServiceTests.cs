using Accounting.Finance.ReceivableVouchers;
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
            docNo1Item.OsAmount.ShouldBe(_testData.DocNo1NativeAmount);

            var docNo2Item = result.Items.First(item => item.DocNo == _testData.DocNo2);
            var docNo2OsAmount = _testData.DocNo2Amount - _testData.DocNo2PaidAmount;
            docNo2Item.PaidAmount.ShouldBe(_testData.DocNo2PaidAmount);
            docNo2Item.PaidNativeAmount.ShouldBe(_testData.DocNo2PaidNativeAmount);
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
        }
    }
}
