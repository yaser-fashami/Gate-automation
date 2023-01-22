using SpmcoBctsDatabaseMigrator.Domain;
using System.Data;
using Microsoft.EntityFrameworkCore;
using SpmcoGateAutomation.Domain.IRepositories;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SpmcoGateAutomation.Domain.DomainServices
{
    [ScopedService]
    public class ManifestDomainService
    {
        private readonly IManifestCntrRepository _manifestCntrRepository;

        public ManifestDomainService(IManifestCntrRepository manifestCntrRepository)
        {
            this._manifestCntrRepository = manifestCntrRepository;
        }
    }
}
