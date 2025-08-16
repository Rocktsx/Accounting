using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Accounting.Finance
{
    public abstract class AccountingPeriodTests
    {
        [Fact]
        public void Can_Create_A_Valid_AccountingPeriod()
        {
            // arrange
            var date = DateTime.Now;
            var startDate = new DateOnly(date.Year, 1, 1);
            var endDate = new DateOnly(date.Year, 12, 31);

            // act
            var entry = new AccountingPeriod(Guid.NewGuid(), date.Year.ToString(), startDate, endDate, true);

            // assert
            entry.ShouldNotBeNull();
            entry.Code.ShouldBe(date.Year.ToString());
            entry.IsCurrentPeriod.ShouldBe(true);
            entry.StartDate.ShouldBe(startDate);
            entry.EndDate.ShouldBe(endDate);
        }
        [Fact]
        public void Can_Not_Create_A_AccountingPeriod_With_Null_Code()
        {
            // arrange
            var date = DateTime.Now;
            // act
            var exception = Assert.Throws<ArgumentException>(() => new AccountingPeriod(Guid.NewGuid(), string.Empty, new DateOnly(date.Year, 1, 1), new DateOnly(date.Year, 12, 31), true));

            // assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }
    }
}
