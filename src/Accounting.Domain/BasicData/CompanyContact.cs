using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Accounting.BasicData
{
    public class CompanyContact : Entity<Guid>
    {
        public Guid CompanyId { get; private set; }
        public Guid ContactId { get; private set; }
        public string Name { get; private set; }
        public string Department { get; private set; }
        public string Position { get; private set; }
        public string DirectLine { get; private set; }
        public string Telephone { get; private set; }
        public string Fax { get; private set; }
        public string Email { get; private set; }
        public string Remark { get; private set; }

        public CompanyContact() { }
        public CompanyContact(Guid companyId, Guid contactId, string name, string department, string position, string directLine, string telephone, string fax, string email, string remark)
        {
            CompanyId = companyId;
            ContactId = contactId;
            SetName(name);
            SetDepartment(department);
            SetPosition(position);
            SetDirectLine(directLine);
            SetTelephone(telephone);
            SetFax(fax);
            SetEmail(email);
            SetRemark(remark);
        }

        public CompanyContact SetName(string name)
        {
            Name = name ?? string.Empty;
            return this;
        }
        public CompanyContact SetDepartment(string department)
        {
            Department = department ?? string.Empty;
            return this;
        }
        public CompanyContact SetPosition(string position)
        {
            Position = position ?? string.Empty;
            return this;
        }
        public CompanyContact SetDirectLine(string directLine)
        {
            DirectLine = directLine ?? string.Empty;
            return this;
        }
        public CompanyContact SetTelephone(string telephone)
        {
            Telephone = telephone ?? string.Empty;
            return this;
        }
        public CompanyContact SetFax(string fax)
        {
            Fax = fax ?? string.Empty;
            return this;
        }
        public CompanyContact SetEmail(string email)
        {
            Email = email ?? string.Empty;
            return this;
        }
        public CompanyContact SetRemark(string remark)
        {
            Remark = remark ?? string.Empty;
            return this;
        }
    }
}
