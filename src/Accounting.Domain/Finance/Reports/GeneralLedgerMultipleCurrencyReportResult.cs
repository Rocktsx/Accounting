using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Finance.Reports
{
    public class GeneralLedgerMultipleCurrencyReportResult: GeneralLedgerSingleCurrencyReportResult
    {
        public string CurrencyCode { get; set; }
        public decimal ForeignAmount { get; set; }
        public GeneralLedgerMultipleCurrencyReportResult()
        {
            CurrencyCode = string.Empty;
        }
    }
}
