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
    public class GeneralTableDomainService
    {
        private readonly IGeneralTableRepository _generalTableRepository;

        public GeneralTableDomainService(IGeneralTableRepository generalTableRepository)
        {
            this._generalTableRepository = generalTableRepository;
        }
        public async Task<List<GeneralTable>> GetGeneralTablesInfoByType(short type)
        {
            var result = await _generalTableRepository.DbSet.Where(c => c.GeneralType == type).ToListAsync();
            return result;
        }
        public async Task<GeneralTable> GetGeneralTablesInfoByTypeAndCode(short type, short code)
        {
            var result = _generalTableRepository.DbSet.Where(c => c.GeneralType == type && c.GeneralCode == code).FirstOrDefault();
            return result;
        }
    }
}
