using Microsoft.EntityFrameworkCore;
using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoBctsDatabaseMigrator.Infrastructure;
using SpmcoGateAutomation.Domain.IRepositories;
using SpmcoGateAutomation.Infrastructure.EFCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Infrastructure.EFCore.IRepositories
{
    public class ActCntrRepository : Repository<ActCntr>, IActCntrRepository
    {
        public virtual DbSet<ActCntr> DbSet { get; set; }
        public ActCntrRepository(BctsDatabaseContext context) : base(context)
        {
            this.DbSet = context.Set<ActCntr>();
        }
    }
}
