using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Common
{
    public interface IGenerateCode
    {
        string Code { get;   }
        string Prefix { get;  }
        int GenNo { get;   }

        void SetCode(string code, string prefix, int GenNo);
    }
}
