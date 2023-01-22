using Microsoft.EntityFrameworkCore;
using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoBctsDatabaseMigrator.Infrastructure;
using SpmcoGateAutomation.Common.Extensions;
using SpmcoGateAutomation.Domain.IRepositories;
using SpmcoGateAutomation.Infrastructure.EFCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Infrastructure.EFCore.IRepositories
{
    public class GateLogRepository : Repository<GateLog>, IGateLogRepository
    {
        public GateLogRepository(BctsDatabaseContext context) : base(context)
        {
            this.Context = context;
            this.DbSet = context.Set<GateLog>();
        }

        public string GetNewConsigneeGateLogNo()
        {
            var year_month = DateTime.Now.ToSmallPersian();
            var LastGateLogNo = (from db in DbSet
                                 where EF.Functions.Like(db.GateLogNo, $"{year_month}%")
                                 select db).OrderByDescending(c => c.GateLogId).FirstOrDefault();
            if (LastGateLogNo == null)
            {
                var number = string.Format("{0:D4}", 1);
                return year_month + number;
            }
            else
            {
                int lastNumber = int.Parse(LastGateLogNo.GateLogNo.Substring(4, 4));
                lastNumber++;
                var number = string.Format("{0:D4}", lastNumber);
                return year_month + number;
            }
        }

    }
}
