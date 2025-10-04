using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.BasicData.Companies
{
    public class CompanyContactCreateDto
    { 
        public Guid Id { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string ContactName { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string Department { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string Position { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string DirectLine { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string Telephone { get; set; }
        [MaxLength(CompanyConsts.CommonMaxLength)]
        public string Fax { get; set; }
        [MaxLength(CompanyConsts.MaxEmailLength)]
        public string Email { get; set; }
        [MaxLength(CompanyConsts.MaxRemarkLength)]
        public string Remark { get; set; }
    }
}
