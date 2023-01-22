using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoGateAutomation.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SpmcoGateAutomation.Domain.IRepositories
{
    [ScopedService]
    public interface IDeliveryPermissionCntrRepository : IRepository<DeliveryPermissionCntr>
    {
    }
}
