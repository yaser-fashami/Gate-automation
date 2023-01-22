using Microsoft.EntityFrameworkCore;
using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoGateAutomation.Domain.IRepositories;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SpmcoGateAutomation.Domain.DomainServices
{
    [ScopedService]
    public class ContainerDomainService
    {
        private readonly IContainerInfoRepository _containerInfoRepository;

        public ContainerDomainService(IContainerInfoRepository containerInfoRepository)
        {
            _containerInfoRepository = containerInfoRepository;
        }
        public async Task<ContainersInfo> GetContainerInfo(string cntrNo)
        {
            var containerInfo = _containerInfoRepository.DbSet.Where(c => c.CntrNo.Equals(cntrNo))
                .Include(c => c.ShippingLine)
                .Include(c => c.ShiShippingLine)
                .Include(c => c.IsoCode)
                .FirstOrDefault();
            return containerInfo;
        }
    }
}
