using Accounting.Finance.Vouchers;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliation : AuditedEntity<Guid>, IMultiTenant //, IHasConcurrencyStamp
    {
        public Guid VoucherDetailId { get; private set; }
        /// <summary>
        /// 是否已兑现
        /// </summary>
        public bool IsPresented { get; private set; }
        public Guid? TenantId { get; private set; }
        //public string ConcurrencyStamp { get; set; }

        public virtual VoucherDetail VoucherDetail { get; private set; }

        private BankReconciliation() { }
        public BankReconciliation(Guid id, Guid voucherDetailId, bool isPresented, Guid? tenantId = null)
        {
            Id= id;
            TenantId = tenantId;
            VoucherDetailId = voucherDetailId;
            IsPresented = isPresented;
        }
        public BankReconciliation SetVoucherDetailId(Guid voucherDetailId)
        {
            VoucherDetailId = voucherDetailId;
            return this;
        }
        public BankReconciliation SetIsPresented(bool isPresented)
        {
            IsPresented = isPresented;
            return this;
        }
    }
}
