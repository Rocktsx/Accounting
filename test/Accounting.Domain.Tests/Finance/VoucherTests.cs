using Shouldly;
using System; 
using System.Linq; 
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class VoucherTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    { 

        private Voucher CreateVoucher(DateOnly voucherDate)
        {
            var voucher = new Voucher(
                Guid.NewGuid(),
                voucherDate,
                VoucherType.JournalVoucher,
                VoucherStatus.Draft
            );

            return voucher;
        }
        private Voucher GetVoucher(DateOnly voucherDate)
        {
            var voucher = CreateVoucher(voucherDate);
            voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
            voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                100.0m, "DOC002", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
            return voucher;
        }
        [Fact]
        public void Can_Create_Voucher()
        {
            // Arrange
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            // Act
            var voucher = GetVoucher(voucherDate);
            // Assert
            voucher.ShouldNotBeNull();
            voucher.VoucherDate.ShouldBe(voucherDate);
            voucher.VoucherType.ShouldBe(VoucherType.JournalVoucher);
            voucher.Status.ShouldBe(VoucherStatus.Draft);
            voucher.Details.Count.ShouldBe(2);
            voucher.Details.Count(item => Guid.Empty.Equals(item.VoucherId)).ShouldBe(0);
        }
        [Fact]
        public void Can_Update_Voucher_Detail()
        {
            // Arrange
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            var voucher = GetVoucher(voucherDate);
            string demoText = "demo";
            Guid subjectId = Guid.NewGuid();
            // Act
            var newVoucherDate = new DateOnly(2024, voucherDate.Month, voucherDate.Day);
            voucher.SetStatus(VoucherStatus.Approval).SetVoucherDate(newVoucherDate).SetVoucherType(VoucherType.PayableVoucher);
            var detailItem = voucher.Details.First();
            voucher.SetDetail(detailItem.Id, subjectId, Guid.Empty, "Test Description33", DebitorCreditor.Creditor, "RMB", 1.1m, 1000.0m,
                1100.0m, "DOC00121", newVoucherDate, demoText, demoText, demoText, demoText, demoText, 1, false);

            // Assert 
            var assertDetailItem = voucher.Details.First(item => item.Id == detailItem.Id);
            voucher.VoucherDate.ShouldBe(newVoucherDate);
            voucher.VoucherType.ShouldBe(VoucherType.PayableVoucher);
            voucher.Status.ShouldBe(VoucherStatus.Approval);
            assertDetailItem.ShouldNotBeNull();
            assertDetailItem.SubjectId.ShouldBe(subjectId);
            assertDetailItem.SubSubjectCode.ShouldBe(Guid.Empty);
            assertDetailItem.DebitorCreditor.ShouldBe(DebitorCreditor.Creditor);
            assertDetailItem.Description.ShouldBe("Test Description33");
            assertDetailItem.CurrencyCode.ShouldBe("RMB");
            assertDetailItem.CurrencyRate.ShouldBe(1.1m);
            assertDetailItem.ForeignAmount.ShouldBe(1000.0m);
            assertDetailItem.NativeAmount.ShouldBe(1100.0m);
            assertDetailItem.DocNo.ShouldBe("DOC00121");
            assertDetailItem.DueDate.ShouldBe(newVoucherDate);
            assertDetailItem.Project.ShouldBe(demoText);
            assertDetailItem.Department.ShouldBe(demoText);
            assertDetailItem.Region.ShouldBe(demoText);
            assertDetailItem.Custom1.ShouldBe(demoText);
            assertDetailItem.Custom2.ShouldBe(demoText);
            assertDetailItem.ItemQty.ShouldBe(1);
            assertDetailItem.IsOriginal.ShouldBe(false);
        }
        [Fact]
        public void Cannot_Update_A_Not_Exists_Voucher_Detail()
        {
            // Arrange
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            var voucher = GetVoucher(voucherDate);

            // Act  
            var detailItemId = Guid.NewGuid();
            voucher.SetDetail(detailItemId, Guid.NewGuid(), null, "Test Description33", DebitorCreditor.Debitor, "RMB", 1.1m, 100.0m,
                110.0m, "DOC0012", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);

            // Assert   
            voucher.Details.ShouldNotContain(item => item.Description == "Test Description33");
            voucher.Details.ShouldNotContain(item => item.CurrencyCode == "RMB");
        }
        [Fact]
        public void Cannot_Add_VoucherDetail_With_Empty_Subject_Id()
        {
            // Arrange 
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            var voucher = CreateVoucher(voucherDate);

            // Act
            var exception = Should.Throw<BusinessException>(() =>
            {
                voucher.AddDetail(Guid.NewGuid(), Guid.Empty, null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
               100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(AccountingDomainErrorCodes.SubjectIdCanNotBeEmpty);
        }
        [Fact]
        public void Cannot_Add_VoucherDetail_With_Empty_Currency_Code()
        {
            // Arrange 
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            var voucher = CreateVoucher(voucherDate);

            // Act
            var exception = Should.Throw<ArgumentException>(() =>
            {
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, string.Empty, 1.0m, 100.0m,
               100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldContain("currencyCode");
        }
        [Fact]
        public void Cannot_Add_VoucherDetail_With_Zero_Currency_Rate()
        {
            // Arrange 
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            var voucher = CreateVoucher(voucherDate);

            // Act
            var exception = Should.Throw<ArgumentException>(() =>
            {
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 0m, 100.0m,
               100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldContain("currencyRate");
        }
        [Fact]
        public void Cannot_Add_VoucherDetail_With_Zero_Foreign_Amount()
        {
            // Arrange 
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            var voucher = CreateVoucher(voucherDate);

            // Act
            var exception = Should.Throw<ArgumentException>(() =>
            {
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 0m,
               100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldContain("foreignAmount");
        }
        [Fact]
        public void Cannot_Add_VoucherDetail_With_Zero_Native_Amount()
        {
            // Arrange 
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            var voucher = CreateVoucher(voucherDate);

            // Act
            var exception = Should.Throw<ArgumentException>(() =>
            {
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100m,
               0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldContain("nativeAmount");
        }
        [Fact]
        public void Cannot_Add_VoucherDetail_With_Not_Match_Amount()
        {
            // Arrange 
            var voucherDate = DateOnly.FromDateTime(DateTime.Now);
            var voucher = CreateVoucher(voucherDate);

            // Act
            var exception = Should.Throw<BusinessException>(() =>
            {
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100m,
               10m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
            });
            // Assert
            exception.ShouldNotBeNull();
            exception.Code.ShouldBe(AccountingDomainErrorCodes.ForeignExchangeRateMatchNativeAmount);
        }
    }
}
