using Microsoft.EntityFrameworkCore;
using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoBctsDatabaseMigrator.Infrastructure;
using SpmcoGateAutomation.Domain.IRepositories;
using SpmcoGateAutomation.Infrastructure.EFCore.Abstracts;

namespace SpmcoGateAutomation.Infrastructure.EFCore.IRepositories
{
    public class DeliveryPermissionCntrRepository : Repository<DeliveryPermissionCntr> , IDeliveryPermissionCntrRepository
    {
        internal BctsDatabaseContext context;
        public virtual DbSet<VoyageInfo> DbSet { get; set; }

        public DeliveryPermissionCntrRepository(BctsDatabaseContext context) : base(context)
        {
            this.context = context;
            this.DbSet = context.Set<VoyageInfo>();
        }
    }
}
