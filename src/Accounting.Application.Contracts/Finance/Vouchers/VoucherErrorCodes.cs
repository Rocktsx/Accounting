using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Finance.Vouchers
{
    public class VoucherErrorCodes
    {
        public const string SubjectCodeCanNotBeEmpty = "Accounting:SubjectCodeCanNotBeEmpty";
        public const string SubjectCodeNotExists = "Accounting:SubjectCodeNotExists";
        public const string SubSubjectCodeNotExists = "Accounting:SubSubjectCodeNotExists";
        public const string DebitAndCreditCanNotBothBeGreaterThanZero = "Accounting:DebitAndCreditCanNotBothBeGreaterThanZero";
        public const string InSingleEntrySubjectCodeCannotBeEmpty = "Accounting:InSingleEntrySubjectCodeCannotBeEmpty";
        public const string CurrencyNotSetUpInSubject = "Accounting:CurrencyNotSetUpInSubject";
        public const string ReceiptAmtGreaterThanSettlementAmt = "Accounting:ReceiptAmtGreaterThanSettlementAmt";
        public const string PaymentReferenceCannotBeEmpty = "Accounting:PaymentReferenceCannotBeEmpty";
        public const string PleaseEnterArSubjectInSetting = "Accounting:PleaseEnterArSubjectInSetting";
    }
}
