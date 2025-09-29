namespace Accounting;

public static class AccountingDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */ 
    public const string AmountsMustNotBeZero = "AmountsMustNotBeZero";
    public const string ExchangeRateMatchAmounts = "ExchangeRateMatchAmounts";
    public const string ForeignExchangeRateMatchNativeAmount = "ForeignExchangeRateMatchNativeAmount";
    public const string VoucherDoesNotBalance = "VoucherDoesNotBalance";
    public const string SubjectIdCanNotBeEmpty = "SubjectIdCanNotBeEmpty";
    public const string VoucherDateIsNotInCurrentPeriodRange = "VoucherDateIsNotInCurrentPeriodRange";
    public const string SubSubjectCodeCanNotBeEmpty = "SubSubjectCodeCanNotBeEmpty";
    public const string DocNoCanNotBeEmpty = "DocNoCanNotBeEmpty";
    public const string DueDateCanNotBeEmpty = "DueDateCanNotBeEmpty";
    public const string CannotFormatVoucherDate = "CannotFormatVoucherDate";
    public const string DocNoIsDuplicated = "DocNoIsDuplicated";
    public const string DocNoHasBeenUsed = "DocNoHasBeenUsed";
}
