using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Accounting.BasicData
{
    public abstract class Company_Tests
    {
        [Fact]
        public  void Can_Create_A_Valid_Company() {  
            // act
            var company = new Company(Guid.NewGuid(),"Test", null,null,null,0 , null, null,true, false);

            //assert
            company.ShouldNotBeNull();
            company.Name.ShouldBe("Test");
            company.OtherName.ShouldBe(string.Empty);
            company.NickName.ShouldBe(string.Empty);
            company.TradeTerm.ShouldBe(string.Empty);
            company.PaymentTerm.ShouldBe(string.Empty);
            company.IsClient.ShouldBeTrue();
            company.IsVendor.ShouldBeFalse();
        }
        [Fact]
        public void Can_Not_Create_A_Company_With_Null_Name()
        {
            // act
            var exception = Assert.Throws<ArgumentException>(() => new Company(Guid.NewGuid(), null, null, null, null, 0, null, null, true, false));
            //assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }
        [Fact]
        public void Can_Add_A_Valid_Address()
        {
            // arrange
            var company = new Company(Guid.NewGuid(), "Test", null, null, null, 0, null, null, true, false);
            //act
            company.AddAddress(Guid.NewGuid(),true,false,"Test",null, null, null, null, null, null, null,null, null);
            //assert
            company.Addresses.Count.ShouldBe(1);
            CompanyAddress address = company.Addresses.First(); 
            address.CompanyId.ShouldBe(company.Id);
            address.Name.ShouldBe("Test");
            address.IsBilling.ShouldBeTrue();
            address.IsShipping.ShouldBeFalse();
            address.Address.ShouldBe(string.Empty);
            address.ContactPerson.ShouldBe(string.Empty);
            address.Telephone.ShouldBe(string.Empty);
            address.Email.ShouldBe(string.Empty);
            address.Remark.ShouldBe(string.Empty);
            address.Country.ShouldBe(string.Empty);
            address.Region.ShouldBe(string.Empty);
            address.District.ShouldBe(string.Empty);
        }
        [Fact]
        public void Can_Update_An_Existsing_Address()
        {
            // arrange
            var company = new Company(Guid.NewGuid(), "Test", null, null, null, 0, null, null, true, false);
            var addressId = Guid.NewGuid();
            company.AddAddress(addressId, true, false, "Test", null, null, null, null, null, null, null, null, null);
            string addr = "1 block";
            //act
            company.SetAddress(addressId, true, true, "Test", addr, null, null, null, null, null, null, null, null);
            //assert
            company.Addresses.Count.ShouldBe(1);
            CompanyAddress address = company.Addresses.First();
            address.Address.ShouldBe(addr);
            address.IsShipping.ShouldBeTrue();
        }
        [Fact]
        public void Can_Add_A_Not_Existsing_Address()
        {
            // arrange
            var company = new Company(Guid.NewGuid(), "Test", null, null, null, 0, null, null, true, false); 
            company.AddAddress(Guid.NewGuid(), true, false, "Test", null, null, null, null, null, null, null, null, null);
            string addr = "1 block";
            //act
            company.SetAddress(Guid.NewGuid(), true, true, "Test2", addr, null, null, null, null, null, null, null, null);
            //assert
            company.Addresses.Count.ShouldBe(2); 
            company.Addresses.ShouldContain(a => a.Name == "Test2");
        }
        [Fact]
        public void Can_Add_A_Valid_Contact()
        {
            // arrange
            var company = new Company(Guid.NewGuid(), "Test", null, null, null, 0, null, null, true, false);
            //act
            company.AddContact(Guid.NewGuid(), "Test", null, null, null, null, null, null, null);
            //assert
            company.Contacts.Count.ShouldBe(1);
            CompanyContact contact = company.Contacts.First();
            contact.CompanyId.ShouldBe(company.Id);
            contact.ContactName.ShouldBe("Test");
            contact.Position.ShouldBe(string.Empty);
            contact.Telephone.ShouldBe(string.Empty);
            contact.Email.ShouldBe(string.Empty);
            contact.Remark.ShouldBe(string.Empty);
        }
        [Fact]
        public void Can_Update_An_Existsing_Contact()
        {
            // arrange
            var company = new Company(Guid.NewGuid(), "Test", null, null, null, 0, null, null, true, false);
            var contactId = Guid.NewGuid();
            company.AddContact(contactId, "Test", null, null, null, null, null, null, null);
            string name = "Test2";
            //act
            company.SetContact(contactId, name, null, null, null, null, null, null, null);
            //assert
            company.Contacts.Count.ShouldBe(1);
            CompanyContact contact = company.Contacts.First();
            contact.ContactName.ShouldBe(name);
        }
        [Fact]
        public void Can_Add_A_Not_Existsing_Contact()
        {
            // arrange
            var company = new Company(Guid.NewGuid(), "Test", null, null, null, 0, null, null, true, false);
            company.AddContact(Guid.NewGuid(), "Test", null, null, null, null, null, null, null);
            string name = "Test2";
            //act
            company.SetContact(Guid.NewGuid(), name, null, null, null, null, null, null, null);
            //assert
            company.Contacts.Count.ShouldBe(2);
            company.Contacts.ShouldContain(c => c.ContactName == name);
        }
    }
}
