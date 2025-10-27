using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;

namespace Accounting.Finance.Vouchers
{
    public class NoVoidVoucherSpecification : Specification<Voucher>
    {
        public override Expression<Func<Voucher, bool>> ToExpression()
        {
            return x => x.Status != VoucherStatus.Void;
        }
    }
}
