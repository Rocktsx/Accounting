namespace Accounting.Web
{
    public static class Helpers
    {
        public static string GetText(string code, string name, string otherName)
        {
            return $"{code} - {name}" + (string.IsNullOrWhiteSpace(otherName) ? "" : $" ({otherName})");
        }
    }
}
