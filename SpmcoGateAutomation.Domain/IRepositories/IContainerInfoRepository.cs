using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoGateAutomation.Domain.Abstracts;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SpmcoGateAutomation.Domain.IRepositories
{
    [ScopedService]
    public interface IContainerInfoRepository : IRepository<ContainersInfo>
    {
    }
}
