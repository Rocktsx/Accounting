using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Common
{
    public class GenerateCode : IGenerateCode
    {
        public string Code { get ; private set ; }
        public string Prefix { get ; private set ; }
        public int GenNo { get ; private set; }
        public GenerateCode()
        {
            Code = string.Empty;
            Prefix = string.Empty;
        }
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
