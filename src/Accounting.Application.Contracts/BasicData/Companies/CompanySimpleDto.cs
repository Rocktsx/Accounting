using Accounting.Dtos;
using System;

namespace Accounting.BasicData.Companies
{
    public class CompanySimpleDto: SimpleDto<Guid>
    {
        public string Currency { get; set; }
    }
}
