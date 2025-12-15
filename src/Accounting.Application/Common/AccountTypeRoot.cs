using Accounting.Finance.AccountTypes;

namespace Accounting.Common
{
    public class AccountTypeRoot
    {
        public AccountType Item { get; set; }
        public AccountType RootItem { get; set; }
        public AccountType SecondaryRootItem { get; set; }
    }
}
