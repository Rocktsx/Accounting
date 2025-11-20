using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Common
{
    public class CodeCacheItem
    {
        public Guid? TenantId { get; set; }
        public FunctionCodes FunctionCode { get; set; }
        public string Prefix { get; set; }
        public int LastNumber { get; set; } 
        public string GetKey()
        {
            return GetKey(TenantId, FunctionCode, Prefix);
        }
        public static string GetKey(Guid? tenantId, FunctionCodes functionCode, string prefix)
        {
            return $"{tenantId ?? Guid.Empty}-{functionCode.ToString()}-{prefix}";
        }
    }
}
