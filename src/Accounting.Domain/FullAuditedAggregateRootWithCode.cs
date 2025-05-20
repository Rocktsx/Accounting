using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting
{
    public class FullAuditedAggregateRootWithCode<T> : FullAuditedAggregateRoot<T>, IGenerateCode
    {
        public string Code { get; private set; }
        public string Prefix { get; private set; }
        public int GenNo { get; private set; } 
       
        public void SetCode(string code, string prefix, int genNo)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(prefix, nameof(prefix));
            Check.Positive(genNo, nameof(genNo));
            Code = code;
            Prefix = prefix;
            GenNo = genNo;
        }
    } 
}
