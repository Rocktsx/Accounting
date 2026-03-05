using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BasicData.Companies
{
    public class CompanyContactDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string ContactName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string DirectLine { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }

        public CompanyContactDto()
        {
            ContactName = string.Empty;
            Department = string.Empty;
            Position = string.Empty;
            DirectLine = string.Empty;
            Telephone = string.Empty;
            Fax = string.Empty;
            Email = string.Empty;
            Remark = string.Empty;
        }
    }
}
