using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Accounting.Finance
{
    public abstract class SubjectTests
    {
        [Fact]
        public void Can_Create_A_Valid_Subject()
        {
            // Act
            var entity = new Subject(Guid.NewGuid(), "1001", "现金", "Cash", null, Guid.NewGuid(),
                CreditDebit.Debit, "CNY", null, true, true, false, 1);
            // Assert
            entity.ShouldNotBeNull();
            entity.Code.ShouldBe("1001");
            entity.Description.ShouldBe(string.Empty);
        }
        [Fact]
        public void Can_Not_Create_A_Subject_With_Null_Code()
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() => new Subject(Guid.NewGuid(), string.Empty, "现金",
                "Cash", null, Guid.NewGuid(), CreditDebit.Debit, "CNY", "This is a cash account", true, true, false, 1
            ));
            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }
        [Fact]
        public void Can_Not_Create_A_Subject_With_Null_Name()
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() => new Subject(Guid.NewGuid(), "1001", string.Empty, "Cash",
                null, Guid.NewGuid(), CreditDebit.Debit, "CNY", "This is a cash account", true, true, false, 1
            ));
            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }
    }
}
