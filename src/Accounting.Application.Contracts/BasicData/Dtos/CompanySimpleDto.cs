using System;

namespace Accounting.BasicData.Dtos
{
    public class CompanySimpleDto: SimpleDto<Guid>
    {
        public string Currency { get; set; }
    }
}
