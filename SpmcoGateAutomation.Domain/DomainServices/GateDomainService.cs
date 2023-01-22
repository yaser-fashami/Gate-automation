using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoGateAutomation.Contract;
using SpmcoGateAutomation.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SpmcoGateAutomation.Domain.DomainServices
{
    [ScopedService]
    public class GateDomainService
    {
        private readonly IDeliveryPermissionCntrRepository _deliveryPermissionCntrRepository;
        private readonly IGateLogRepository _gateLogRepository;

        public GateDomainService(
            IDeliveryPermissionCntrRepository deliveryPermissionCntrRepository,
            IGateLogRepository gateLogRepository)
        {
            this._deliveryPermissionCntrRepository = deliveryPermissionCntrRepository;
            this._gateLogRepository = gateLogRepository;
        }
        public async Task<DeliveryPermissionCntr> GetDeliveryPermissionCntrInfoByPlateNo(string plateNo)
        {
            DateTime DateNow = DateTime.Now.Date;
            var dpcInfo = _deliveryPermissionCntrRepository.DbSet.
                Where(c => c.TruckNo.Equals(plateNo) && !c.IsDeleted)
                .Include(dpc => dpc.DeliveryPermission).Where(dpc => dpc.DeliveryPermission.DeliveryPermissionDue >= DateNow && !dpc.IsDeleted)
                .Include(dpc => dpc.ReceiptCntrConsignee).ThenInclude(rc => rc.Blcntr).ThenInclude(bc => bc.ManifestCntr)
                .Include(dpc => dpc.GateLog).OrderByDescending(dpc => dpc.DeliveryPermissionId).FirstOrDefault();
            return dpcInfo;
        }
        public async Task<GateLog> GetGateInfoByPlateNoAndCntrNo(string plateNo, string cntrNo)
        {
            DateTime DateNow = DateTime.Now.Date;
            DateTime TwentyFourHoursAgo = DateTime.Now.AddHours(-24);
            var gateInInfo = _gateLogRepository.DbSet.Where(c => c.TruckNo.Equals(plateNo) &&
                                                           (c.CntrNoGateIn.Equals(cntrNo) || EF.Functions.Like(c.CntrNoGateIn, $"%{cntrNo.Substring(5, 7)}") 
                                                           && c.GateInTime >= TwentyFourHoursAgo
                                                           ))
                .Include(gl => gl.DeliveryPermissionCntr).ThenInclude(dpc => dpc.DeliveryPermission).Where(gl => 
                gl.DeliveryPermissionCntr.DeliveryPermission.DeliveryPermissionDue >= DateNow && 
                !gl.DeliveryPermissionCntr.IsDeleted && !gl.DeliveryPermissionCntr.DeliveryPermission.IsDeleted)
                .Include(gl => gl.DeliveryPermissionCntr).ThenInclude(dpc => dpc.ExitPermitCntrs.Where(epc => !epc.IsDeleted)).ThenInclude(epc => epc.ExitPermit).FirstOrDefault();
            //.OrderByDescending(c => c.GateLogId)
            //.FirstOrDefault();
            return gateInInfo;
        }
        public async Task<GateLog> CreateTruckOfConsigneeGateInLog(string cntrNo, string plateNo, long dpcId, string imageName, long gateInNo, byte logType)
        {
            var GateLogNo = _gateLogRepository.GetNewConsigneeGateLogNo();
            return new GateLog
            {
                CntrNoGateIn = cntrNo,
                GateInTime = DateTime.Now,
                DeliveryPermissionCntrId = dpcId,
                TruckNo = plateNo,
                LogType = logType,
                GateInImageName = imageName,
                GateLogNo = GateLogNo,
                IsDeleted = false,
                GateInOperator = 220,
                GateInId = gateInNo
            };
        }
        public async Task<GateLog> CreateTruckOfConsigneeGateOutLog(long gateLogId, string imageName, long gateOutNo, byte logType)
        {
            var gateLogInfo = await _gateLogRepository.FindByIdAsync(gateLogId);
            gateLogInfo.CntrNoGateOut = gateLogInfo.CntrNoGateIn;
            gateLogInfo.GateOutImageName = imageName;
            gateLogInfo.GateOutId = gateOutNo;
            gateLogInfo.LogType = logType;
            gateLogInfo.GateOutTime = DateTime.Now;
            gateLogInfo.GateOutOperator = 220;
            return gateLogInfo;
        }
    }
}
