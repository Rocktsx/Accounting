using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Accounting.Finance
{
    public abstract class AccountTypeTests
    {
        [Fact]
        public void Can_Create_A_Valid_AccountType()
        {
            // Arrange & Act
           var entity = new AccountType("A", "資產", "Assets",string.Empty,1,0,1,1,0,1);
            // Assert
            entity.ShouldNotBeNull();
            entity.Id.ShouldBe("A");
            entity.Name.ShouldBe("資產");
            entity.OtherName.ShouldBe("Assets");
            entity.ParentId.ShouldBe(string.Empty);
            entity.TrialBalanceSort.ShouldBe(1);
            entity.ProfitAndLossSort.ShouldBe(0);
            entity.BalanceSheetSort.ShouldBe(1);
            entity.TrialBalanceGroup.ShouldBe(1);
            entity.ProfitAndLossGroup.ShouldBe(0);
            entity.BalanceSheetGroup.ShouldBe(1); 
        } 
        [Fact]
        public void Cannot_Create_A_AccountType_Without_Name()
        {
            // Arrange & Act
            var exception = Should.Throw<ArgumentException>(() => new AccountType("A", string.Empty, "Assets", string.Empty, 1, 0, 1, 1, 0, 1));
            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }
    }
}
