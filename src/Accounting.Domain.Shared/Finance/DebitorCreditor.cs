
namespace Accounting.Finance
{
    public enum DebitorCreditor
    {
        Creditor = -1,
        Debitor = 1
    }

    public class DebitorCreditorConst
    {
        public const string Creditor = "CR";
        public const string Debitor = "DR";
    }
    public static class DebitorCreditors
    {
        public static DebitorCreditor Reverse(this DebitorCreditor debitorCreditor)
        {
            return debitorCreditor == DebitorCreditor.Creditor ?
                DebitorCreditor.Debitor : DebitorCreditor.Creditor;
        }
        public static string GetShortName(this DebitorCreditor debitorCreditor)
        {
            return debitorCreditor == DebitorCreditor.Creditor ?
                DebitorCreditorConst.Creditor : DebitorCreditorConst.Debitor;
        }

        public static bool IsDebitor(this DebitorCreditor debitorCreditor)
        {
            return debitorCreditor == DebitorCreditor.Debitor;
        }
        public static bool IsCreditor(this DebitorCreditor debitorCreditor)
        {
            return debitorCreditor == DebitorCreditor.Creditor;
        }
    }
}
