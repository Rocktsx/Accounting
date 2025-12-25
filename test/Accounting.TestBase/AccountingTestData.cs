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
        public Guid AccountingPeriodYearId { get; } = Guid.NewGuid();
        public int AccountingPeriodYear { get; } = 2025;
        public DateOnly AccountingPeriodStartDate { get; } = new DateOnly(2025, 1, 1);
        public DateOnly AccountingPeriodEndDate { get; } = new DateOnly(2025, 12, 31);
        public Guid AccountingPeriod2024YearId { get; } = Guid.NewGuid();
        public int AccountingPeriod2024Year { get; } = 2024;
        public string AccountingPeriod2024Code { get; } = "2024-Updated";
        public int InsertedVouchers { get; } = 7;
        public int InsertedJournalVouchers { get; } = 5;
        public int InsertedReceivableVouchers { get; } = 1;
        public int InsertedPayableVouchers { get; } = 1;
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
        public Guid SubjectBankId { get; } = Guid.NewGuid();
        public string SubjectBankCode { get; } = "2801";
        public string SubjectBankName { get; } = "銀行 (往來戶口）";
        public string SubjectBankOtherName { get; } = "Bank (C/A)"; 
        public string AccountTypeAex { get; } = "AEX";
        public string AccountTypeA { get; } = "A";
        public string AccountTypeCA { get; } = "CA";
        public string AccountTypeE { get; } = "E";
        public string AccountTypeAEX { get; } = "AEX";
        public Guid SubjectRentId { get; } = Guid.NewGuid();
        public string SubjectRentCode { get; } = "8021";
        public string SubjectRentName { get; } = "租金及差餉";
        public string SubjectRentOtherName { get; } = "Rent & Rates";

        public string AccountTypeAr { get; } = "AR";
        public Guid SubjectArId { get; } = Guid.NewGuid();
        public string SubjectArCode { get; } = "25";
        public string SubjectArName { get; } = "应收账款";
        public string SubjectArOtherName { get; } = "Trade Receivables";
        public string AccountTypeAp { get; } = "AP";
        public Guid SubjectApId { get; } = Guid.NewGuid();
        public string SubjectApCode { get; } = "42";
        public string SubjectApName { get; } = "应付账款";
        public string SubjectApOtherName { get; } = "Trade Payables";
        public string AccountTypeNa { get; } = "NA";
        public Guid SubjectCategoryId { get; } = Guid.NewGuid();
        public string SubjectCategoryCode { get; } = "1";
        public string SubjectCategoryName { get; } = "非流动资产";
        public string SubjectCategoryOtherName { get; } = "Non-Current Assets";

        public string VoucherCode { get; } = "JV-0001";
        public string VoucherPrefix { get; } = "JV";
        public string VoucherPrefixTv { get; } = "TV";
        public string VoucherDescription { get; } = "Rent & Rates 2011 01";
        public string VoucherDescription3 { get; } = "Rent & Rates 2025 01";
        public string True { get; } = "true";
        public string DocNo1 { get; } = "INV-0001";
        public decimal DocNo1NativeAmount { get; } = 12600.0m;
        public string DocNo2 { get; } = "INV-0002";
        public decimal DocNo2Amount { get; } = 1000m;
        public decimal DocNo2NativeAmount { get; } = 7200m;
        public decimal DocNo2PaidAmount { get; } = 100m;
        public decimal DocNo2PaidNativeAmount { get; } = 720m;
        public string VoucherDescription2 { get; } = "Test Receivalbe";
        public string VoucherCode2 { get; } = "JV-0002";
        public string VoucherCode3 { get; } = "JV-0003";
        public string VoucherCode4{ get; } = "JV-0004";
        public string VoucherCode5 { get; } = "JV-0005";
        public string VoucherRvCode { get; } = "RV-0001";
        public string VoucherRvPrefix { get; } = "RV";
        public decimal UsdCurrencyRate { get; } = 7.2m;
        public Guid VoucherRvId { get; } = Guid.NewGuid();
        public string PaymentReference { get; } = "Ref0001";
        public string DocNo3 { get; } = "PI-0001";
        public string DocNo4 { get; } = "PI-0002";
        public string VoucherPvCode { get; } = "PV-0001";
        public string VoucherPvPrefix { get; } = "PV";
        public Guid VoucherPvId { get; } = Guid.NewGuid();

        public int InsertedSubjectCount { get; } = 4;
        public int AgingDays { get; } = 7;
        public Guid VoucherDetailId { get; } = Guid.NewGuid();
        public Guid VoucherDetailId2 { get; } = Guid.NewGuid();
        public Guid BankReconciliationId { get; } = Guid.NewGuid();
    }
}
