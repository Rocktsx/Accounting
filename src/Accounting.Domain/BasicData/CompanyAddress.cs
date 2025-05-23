using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Accounting.BasicData
{
    public class CompanyAddress : Entity<Guid>
    {
        public Guid CompanyId { get; private set; }
        public Guid AddressId { get; private set; }
        public bool IsBilling { get; private set; }
        public bool IsShipping { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string ContactPerson { get; private set; }
        public string Telephone { get; private set; }
        public string Email { get; private set; }
        public string Remark { get; private set; }
        public string Country { get; private set; }
        public string Region { get; private set; }
        public string District { get; private set; } 

        private CompanyAddress() { }
        internal CompanyAddress(Guid companyId, Guid addressId, bool isBilling, bool isShipping, string name, string address, string contactPerson, string telephone, string email, string remark, string country, string region, string district)
        {
            CompanyId = companyId;
            AddressId = addressId;
            IsBilling = isBilling;
            IsShipping = isShipping;

            SetAddress(address);
            SetName(name);
            SetContactPerson(contactPerson);
            SetCountry(country);
            SetRegion(region);
            SetDistrict(district);
            SetTelephone(telephone);
            SetEmail(email);
            SetRemark(remark); 
        } 
        public CompanyAddress SetIsBilling(bool isBilling)
        {
            IsBilling = isBilling;
            return this;
        }
        public CompanyAddress SetIsShipping(bool isShipping)
        {
            IsShipping = isShipping;
            return this;
        }
        public CompanyAddress SetName(string name)
        {
            Name = name ?? string.Empty;
            return this;
        }
        public CompanyAddress SetAddress(string address)
        {
            Address = address ?? string.Empty;
            return this;
        }
        public CompanyAddress SetContactPerson(string contactPerson)
        {
            ContactPerson = contactPerson ?? string.Empty;
            return this;
        }
        public CompanyAddress SetTelephone(string telephone)
        {
            Telephone = telephone ?? string.Empty;
            return this;
        }
        public CompanyAddress SetEmail(string email)
        {
            Email = email ?? string.Empty;
            return this;
        }
        public CompanyAddress SetRemark(string remark)
        {
            Remark = remark ?? string.Empty;
            return this;
        }
        public CompanyAddress SetCountry(string country)
        {
            Country = country ?? string.Empty;
            return this;
        }
        public CompanyAddress SetRegion(string region)
        {
            Region = region ?? string.Empty;
            return this;
        }
        public CompanyAddress SetDistrict(string district)
        {
            District = district??string.Empty;
            return this;
        }
    }
}
