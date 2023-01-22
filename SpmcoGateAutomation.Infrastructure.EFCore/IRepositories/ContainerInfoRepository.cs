using Microsoft.EntityFrameworkCore;
using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoBctsDatabaseMigrator.Infrastructure;
using SpmcoGateAutomation.Domain.IRepositories;
using SpmcoGateAutomation.Infrastructure.EFCore.Abstracts;

namespace SpmcoGateAutomation.Infrastructure.EFCore.IRepositories
{
    public class ContainerInfoRepository : Repository<ContainersInfo>,IContainerInfoRepository
    {
        internal BctsDatabaseContext context;
        public virtual DbSet<ContainersInfo> DbSet { get; set; }

        public ContainerInfoRepository(BctsDatabaseContext context) : base(context)
        {
            this.context = context;
            this.DbSet = context.Set<ContainersInfo>();
        }
    }
}
