using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;

namespace Accounting.BasicData
{
    public class Company : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public string Name { get; private set; }
        public string OtherName { get; private set; }

        public string NickName { get; private set; }
        public string Currency {  get; private set; }
        public decimal CreditLimit { get; private set; }
        public string PaymentTerm { get; private set; }
        public string TradeTerm { get; private set; }
        public bool IsClient { get; private set; }
        public bool IsVendor { get; private set; }

        public Guid? TenantId { get; set; }

        public ICollection<CompanyAddress> Addresses { get; set; } = new List<CompanyAddress>();
        public ICollection<CompanyContact> Contacts { get; set; } = new List<CompanyContact>();
        public Company() { }
        public Company(string name, string otherName, string nickName, string currency, decimal creditLimit, string paymentTerm, string tradeTerm, bool isClient, bool isVendor)
        { 
            SetName(name);
            SetNickName(nickName);
            SetOtherName(otherName);
            SetPaymentTerm(paymentTerm);
            SetTradeTerm(tradeTerm);
            SetCurrency(currency);
            CreditLimit = creditLimit;
            IsClient = isClient;
            IsVendor = isVendor; 
        }
        public Company SetName(string name) { 
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Name = name; 
            return this;
        }
        public Company SetOtherName(string otherName) { 
            OtherName = otherName?? string.Empty;
            return this;
        }
        public Company SetNickName(string nickName) { 
            NickName=nickName?? string.Empty;
            return this;
        }
        public Company SetCurrency(string currency) { 
            Currency = currency ?? string.Empty;
            return this;
        }
        public Company SetPaymentTerm(string paymentTerm) { 
            PaymentTerm = paymentTerm ?? string.Empty; 
            return this; 
        }
        public Company SetTradeTerm(string tradeTerm) { 
            TradeTerm=tradeTerm ?? string.Empty;
            return this;
        }
        public Company SetCreditLimit(decimal creditLimit)
        {
            CreditLimit = creditLimit;
            return this;
        }
        public Company SetIsClient(bool isClient)
        {
            IsClient = isClient;
            return this;
        }
        public Company SetIsVendor(bool isVendor)
        {
            IsVendor = isVendor;
            return this;
        }
    } 
}
