using Microsoft.EntityFrameworkCore;
using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoGateAutomation.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Domain.DomainServices
{
    public class ActCntrDomainService
    {
        private readonly IActCntrRepository _actCntrRepository;

        public ActCntrDomainService(IActCntrRepository actCntrRepository)
        {
            this._actCntrRepository = actCntrRepository;
        }
        public async Task<ActCntr> GetLastActCntr(string cntrNo)
        {
            var result = _actCntrRepository.DbSet
                .Where(c => c.CntrNo == cntrNo)
                .Include(c=>c.Equipment)
                .OrderByDescending(c => c.ActId)
                .FirstOrDefault();
            return result;
        }
        public async Task<ActCntr> GetLastActCntrByVoyage(string cntrNo,long voyageId)
        {
            var result = _actCntrRepository.DbSet
                .Where(c => c.CntrNo == cntrNo && c.VoyageId == voyageId)
                .Include(c => c.Equipment)
                .OrderByDescending(c => c.CntrNo)
                .FirstOrDefault();
            return result;
        }
        public async Task<ActCntr> GetLastEquipmentOnLocation(string location)
        {
            var result = _actCntrRepository.DbSet
                .Where(c => c.CntrLocation.Contains(location))
                .Include(c => c.Equipment)
                .OrderByDescending(c => c.ActId).FirstOrDefault();
            return result;
        }

    }
}
