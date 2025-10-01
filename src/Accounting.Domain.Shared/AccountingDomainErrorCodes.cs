namespace Accounting;

public static class AccountingDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */ 
    public const string AmountsMustNotBeZero = "Accounting:AmountsMustNotBeZero";
    public const string ExchangeRateMatchAmounts = "Accounting:ExchangeRateMatchAmounts";

    public const string ForeignExchangeRateMatchNativeAmount = "Accounting:ForeignExchangeRateMatchNativeAmount";
    public const string VoucherDoesNotBalance = "Accounting:VoucherDoesNotBalance";
    public const string SubjectIdCanNotBeEmpty = "Accounting:SubjectIdCanNotBeEmpty";
    public const string VoucherDateIsNotInCurrentPeriodRange = "Accounting:VoucherDateIsNotInCurrentPeriodRange";
    public const string SubSubjectCodeCanNotBeEmpty = "Accounting:SubSubjectCodeCanNotBeEmpty";
    public const string DocNoCanNotBeEmpty = "Accounting:DocNoCanNotBeEmpty";
    public const string DueDateCanNotBeEmpty = "Accounting:DueDateCanNotBeEmpty";
    public const string CannotFormatVoucherDate = "Accounting:CannotFormatVoucherDate";
    public const string DocNoIsDuplicated = "Accounting:DocNoIsDuplicated";
    public const string DocNoHasBeenUsed = "Accounting:DocNoHasBeenUsed";

    public static class Subjects
    { 
        public const string SubjectIsInUse = "Accounting:SubjectIsInUse";
    }
    public const string CodeIsDuplicated = "Accounting:CodeIsDuplicated";
    public const string CodeIsInUse = "Accounting:CodeIsInUse";
}
