using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Accounting
{
    public class AccountingTestData : ISingletonDependency
    {
        public Guid ClientId { get; } = Guid.NewGuid();
        public string ClientCode { get; } = "A-SUNKIST";
        public string ClientName { get; } = "SUNKIST (FAR EAST) PROMOTION LTD.";
        public string ClientOtherName { get; } = "SUNKIST (FAR EAST) PROMOTION LTD.";
        public string ClientNickName { get; } = "A-SUNKIST";
        public string ClientAddressName { get; } = "MANAGING DIRECTOR";
        public string ClientContactDeparment = "Sales Deparment";
        public string ClientContactPosition { get; } = "Sale";
        public string ClientAddress { get; } = "1303 BANK OF AMERICA TOWER12 HARCOURT ROADCENTRAL";
        public string ClientAddressContactPerson { get; } = "MARIA KWOK";
        public string ClientAddressTelephone { get; } = "28453454";

        public string ClientAddressEmail { get; } = "SZ@SZ.COM";
        public string ClientAddressFax { get; } = "28453454";
        public string ClientAddressCountry { get; } = "China";
        public string ClientAddressRegion{ get; } = "SZ";
        public int AccountingPeriodYear { get; } = 2025;
        public DateOnly AccountingPeriodStartDate { get; } = new DateOnly(2025, 1, 1);
        public DateOnly AccountingPeriodEndDate { get; } = new DateOnly(2025, 1, 1);
        public int AccountingPeriod2024Year { get; } = 2024;
        public string AccountingPeriod2024Code { get; } = "2024-Updated";

        public Guid VendorId { get; } = Guid.NewGuid();
        public string VendorCode { get; } = "B-TEST";
        public string VendorName { get; } = "TEST COMPANY LTD.";
        public string VendorOtherName { get; } = "TEST COMPANY LTD.";
        public string VendorNickName { get; } = "B-TEST";
        public string RmbCurrency { get; } = "RMB";
        public string UsdCurrency { get; } = "USD";
        public string AccountTypeBank { get; } = "BAK";
        public string AccountTypeCashCode{ get; } = "1000";
        public string AccountTypeCashName { get; } = "Cash";
        public Guid Subject2801Id { get; } = Guid.NewGuid();
        public string Subject2801Code { get; } = "2801";
        public string Subject2801Name { get; } = "銀行 (往來戶口）";
        public string Subject2801OtherName { get; } = "Bank (C/A)"; 
        public string AccountTypeAex { get; } = "AEX";
        public Guid Subject8021Id { get; } = Guid.NewGuid();
        public string Subject8021Code { get; } = "8021";
        public string Subject8021Name { get; } = "租金及差餉";
        public string Subject8021OtherName { get; } = "Rent & Rates";

        public string AccountTypeAr { get; } = "AR";
        public Guid Subject25Id { get; } = Guid.NewGuid();
        public string Subject25Code { get; } = "25";
        public string Subject25Name { get; } = "应收账款";
        public string Subject25OtherName { get; } = "Trade Receivables";
        public string AccountTypeAp { get; } = "AP";
        public Guid Subject42Id { get; } = Guid.NewGuid();
        public string Subject42Code { get; } = "42";
        public string Subject42Name { get; } = "应付账款";
        public string Subject42OtherName { get; } = "Trade Payables";
        public string AccountTypeNa { get; } = "NA";
        public Guid SubjectCategoryId { get; } = Guid.NewGuid();
        public string SubjectCategoryCode { get; } = "1";
        public string SubjectCategoryName { get; } = "非流动资产";
        public string SubjectCategoryOtherName { get; } = "Non-Current Assets";

        public string VoucherCode { get; } = "JV-0001";
        public string VoucherPrefx { get; } = "JV";
        public string VoucherDescription { get; } = "Rent & Rates 2011 01";
    }
}
