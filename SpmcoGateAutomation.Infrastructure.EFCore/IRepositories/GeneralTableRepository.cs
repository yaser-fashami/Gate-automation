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
    public class GeneralTableRepository : Repository<GeneralTable>, IGeneralTableRepository
    {
        internal BctsDatabaseContext context;
        public virtual DbSet<GeneralTable> DbSet { get; set; }

        public GeneralTableRepository(BctsDatabaseContext context) : base(context)
        {
            this.context = context;
            this.DbSet = context.Set<GeneralTable>();
        }
    }
}
