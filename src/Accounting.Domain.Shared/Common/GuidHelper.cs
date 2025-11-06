using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Common
{
    public static class GuidHelper
    {
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }
        public static bool IsEmptyOrNull(this Guid? guid)
        {
            return guid == null ||  guid == Guid.Empty;
        }
    }
}
