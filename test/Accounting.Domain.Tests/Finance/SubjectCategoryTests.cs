using Shouldly;
using System;
using Xunit;

namespace Accounting.Finance
{
    public abstract class SubjectCategoryTests
    {
        [Fact]
        public void Can_Create_A_Valid_SubjectCategory()
        {
            // Arrange
            var entity = new SubjectCategory(
                Guid.NewGuid(), "1001", "现金", "Cash", null, DebitorCreditor.Debitor, Guid.NewGuid(), true, description: null
            );
            // Act & Assert
            entity.ShouldNotBeNull();
            entity.Code.ShouldBe("1001");
            entity.Description.ShouldBe(string.Empty);
        }
        [Fact]
        public void Can_Not_Create_A_SubjectCategory_With_Null_Code()
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() => new SubjectCategory(
                Guid.NewGuid(), string.Empty, "现金", "Cash", null, DebitorCreditor.Debitor, Guid.NewGuid(), true, "This is a cash account"
            ));
            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Can_Not_Create_A_SubjectCategory_With_Null_Name()
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() => new SubjectCategory(
                Guid.NewGuid(), "1001", string.Empty, "Cash", null, DebitorCreditor.Debitor, Guid.NewGuid(), true, "This is a cash account"
            ));
            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }
    }
}
