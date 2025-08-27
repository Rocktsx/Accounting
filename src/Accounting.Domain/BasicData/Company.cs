using System;
using System.Collections.Generic;
using System.Linq; 
using Volo.Abp; 
using Volo.Abp.MultiTenancy;

namespace Accounting.BasicData
{
    public class Company : AuditedAggregateRootWithCode<Guid>, IMultiTenant
    {
        public string Name { get; private set; }
        public string OtherName { get; private set; }

        public string NickName { get; private set; }
        public string Currency { get; private set; }
        public decimal CreditLimit { get; private set; }
        public string PaymentTerm { get; private set; }
        public string TradeTerm { get; private set; }
        public bool IsClient { get; private set; }
        public bool IsVendor { get; private set; }

        public Guid? TenantId { get; set; }

        public virtual ICollection<CompanyAddress> Addresses { get; private  set; } = [];
        public virtual ICollection<CompanyContact> Contacts { get; private set; } = [];
        private Company() { }
        public Company(Guid id, string name, string otherName, string nickName, string currency, decimal creditLimit, string paymentTerm, string tradeTerm, bool isClient, bool isVendor)
        {
            Check.NotNull(id, nameof(id));
            Id = id;
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
        public Company SetName(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Name = name;
            return this;
        }
        public Company SetOtherName(string otherName)
        {
            OtherName = otherName ?? string.Empty;
            return this;
        }
        public Company SetNickName(string nickName)
        {
            NickName = nickName ?? string.Empty;
            return this;
        }
        public Company SetCurrency(string currency)
        {
            Currency = currency ?? string.Empty;
            return this;
        }
        public Company SetPaymentTerm(string paymentTerm)
        {
            PaymentTerm = paymentTerm ?? string.Empty;
            return this;
        }
        public Company SetTradeTerm(string tradeTerm)
        {
            TradeTerm = tradeTerm ?? string.Empty;
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
        public Company AddAddress(Guid addressId, bool isBilling, bool isShipping, string name, string address, string contactPerson, string telephone, string email, string remark, string country, string region, string district, string fax)
        {
            var companyAddress = new CompanyAddress(Id, addressId, isBilling, isShipping, name, address, contactPerson, telephone, email, remark, country, region, district, fax);
            Addresses.Add(companyAddress);
            return this;
        }
        public Company SetAddress(Guid addressId, bool isBilling, bool isShipping, string name, string address, string contactPerson, string telephone, string email, string remark, string country, string region, string district, string fax)
        {
            var companyAddress = Addresses.FirstOrDefault(a => a.Id == addressId);
            if (companyAddress == null)
            {
                return this;
            }
            companyAddress.SetName(name)
                    .SetAddress(address)
                    .SetContactPerson(contactPerson)
                    .SetTelephone(telephone)
                    .SetEmail(email)
                    .SetRemark(remark)
                    .SetCountry(country)
                    .SetRegion(region)
                    .SetDistrict(district)
                    .SetIsBilling(isBilling)
                    .SetIsShipping(isShipping)
                    .SetFax(fax);
            return this;
        }
        public Company AddContact(Guid contactId, string name, string department, string position, string directLine, string telephone, string fax, string email, string remark)
        {
            var companyContact = new CompanyContact(Id, contactId, name, department, position, directLine, telephone, fax, email, remark);
            Contacts.Add(companyContact);
            return this;
        }
        public Company SetContact(Guid contactId, string name, string department, string position, string directLine, string telephone, string fax, string email, string remark)
        {
            var companyContact = Contacts.FirstOrDefault(c => c.Id == contactId);
            if (companyContact == null)
            {
                return this;
            }
            companyContact.SetContactName(name)
                     .SetDepartment(department)
                     .SetPosition(position)
                     .SetDirectLine(directLine)
                     .SetTelephone(telephone)
                     .SetFax(fax)
                     .SetEmail(email)
                     .SetRemark(remark);
            return this;
        }
    }
}
