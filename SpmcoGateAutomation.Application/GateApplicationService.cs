using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using SpmcoBctsDatabaseMigrator.Domain;
using SpmcoGateAutomation.Common.Configurations;
using SpmcoGateAutomation.Common.Enums;
using SpmcoGateAutomation.Common.Extensions;
using SpmcoGateAutomation.Contract;
using SpmcoGateAutomation.Domain.Abstracts;
using SpmcoGateAutomation.Domain.DomainServices;
using SpmcoGateAutomation.Domain.IRepositories;
using SpmcoGateAutomation.ExternalServices;
using System.Diagnostics.CodeAnalysis;

namespace SpmcoGateAutomation.Application
{
    public class GateApplicationService : IGateApplicationService
    {
        private readonly ILogger<GateApplicationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ContainerDomainService _containerDomainService;
        private readonly GateDomainService _gateDomainService;
        private readonly GeneralTableDomainService _generalTableDomainService;
        private readonly ActCntrDomainService _actCntrDomainService;
        private readonly IGateLogRepository _gateLogRepository;
        private readonly LightService _lightService;
        private readonly GateConfiguration _gateConfiguration;
        private readonly PrinterService _printerService;

        public GateApplicationService(
            ILogger<GateApplicationService> logger,
            IUnitOfWork unitOfWork, 
            ContainerDomainService containerDomainService, 
            GateDomainService gateDomainService,
            GeneralTableDomainService generalTableDomainService,
            ActCntrDomainService actCntrDomainService,
            IGateLogRepository gateLogRepository,
            LightService lightService,
            GateConfiguration gateConfiguration,
            PrinterService printerService)
        {
            this._logger = logger;
            this._unitOfWork = unitOfWork;
            this._containerDomainService = containerDomainService;
            this._gateDomainService = gateDomainService;
            this._generalTableDomainService = generalTableDomainService;
            this._actCntrDomainService = actCntrDomainService;
            this._gateLogRepository = gateLogRepository;
            this._lightService = lightService;
            this._gateConfiguration = gateConfiguration;
            this._printerService = printerService;
        }

        public async Task<bool> SavingTruckOfConsigneeGateInOperation(CreateGateInDto createGateInDto)
        {
            var result = false;
            try
            {
                var dpcInfo = await _gateDomainService.GetDeliveryPermissionCntrInfoByPlateNo(createGateInDto.PlateNo);

                Func<GateLog,Task<bool>> PrintingTruckOfConsigneeGateInOperation = async(GateLog gateLog) =>
                {
                    if (dpcInfo == null) return false;
                    var cntrNo = dpcInfo.ReceiptCntrConsignee.Blcntr.ManifestCntr.CntrNo;
                    var cntrInfo = await _containerDomainService.GetContainerInfo(cntrNo);
                    var lastActInfo = await _actCntrDomainService.GetLastActCntr(cntrNo);
                    var cntrType = await _generalTableDomainService.GetGeneralTablesInfoByTypeAndCode((short)GeneralTypes.ContainerType, cntrInfo.IsoCode.ContainerType);
                    string equipmentName = "";
                    if (lastActInfo.CntrLocation != "" && lastActInfo.CntrLocation != null && lastActInfo.CntrLocation.Length >= 2)
                    {
                        var act = await _actCntrDomainService.GetLastEquipmentOnLocation(lastActInfo.CntrLocation.Substring(0,2));
                        if (act != null && act.Equipment != null)
                        {
                            equipmentName = act.Equipment.EquipmentName;
                        }
                    }
                    await _printerService.PrintTruckOfConsigneeGateInInfo(new ConsigneeGateInPrinterRequestDto
                    {
                        DeliveryPermissionNo = dpcInfo.DeliveryPermission.DeliveryPermissionNo,
                        CntrNo = dpcInfo.ReceiptCntrConsignee.Blcntr.ManifestCntr.CntrNo,
                        CntrLocation = lastActInfo.CntrLocation,
                        Code = gateLog.GateLogNo,
                        GateDate = gateLog.GateInTime.ToPersianDateTime(),
                        GateNo = $"Gate-In{_gateConfiguration.ConsigneeGateInNo}",
                        SizeType = $"{cntrInfo.IsoCode.CntrSize} {cntrType.GeneralName}",
                        TruckNo = gateLog.TruckNo,
                        equipmentName = equipmentName,
                        GateOperator="",
                        yardOperatorName="",
                        GateLogNo= gateLog.GateLogNo
                    });
                    return true;
                };

                if (dpcInfo == null)
                {
                    //_gateInHubSignalRService
                    //    .SendGateInMessageToClients(GateMessagesTemplate.ValidDeliveryPermissionHasNotBeenFound.GetEnumDescription());
                    _logger.LogError(Enum.GetName<GateMessagesTemplate>(GateMessagesTemplate.ValidDeliveryPermissionHasNotBeenFound));
                    return false;
                } // When CntrNo has been gated in already
                else if (dpcInfo.GateLog != null && dpcInfo.GateLog.GateLogId > 0)
                {

                    //_gateInHubSignalRService
                    //    .SendGateInMessageToClients(GateMessagesTemplate.GateInInfoHasBeenSavedAlready.GetEnumDescription());
                    _logger.LogError(Enum.GetName<GateMessagesTemplate>(GateMessagesTemplate.GateInInfoHasBeenSavedAlready));
                    //_gateInImageDto.IsFinished = true;
                    //we will print duplicate just when this container has not been gted out --------------
                    try
                    {
                        await PrintingTruckOfConsigneeGateInOperation(dpcInfo.GateLog);
                        var isTurnedOn = await _lightService.TurningOnConsigneeGateInLight();
                        if (isTurnedOn)
                        {
                            Thread.Sleep(int.Parse(_gateConfiguration.ConsigneeDelayTimeLight));
                            await _lightService.TurningOffConsigneeGateInLight();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        //Console.WriteLine(ex);
                    }
                    return false;
                    //-------------------------------------------------------------------------------------
                } // When CntrNo has not been gateIn yet , we will save gate In
                else
                {
                    var cntrNo = dpcInfo.ReceiptCntrConsignee.Blcntr.ManifestCntr.CntrNo;
                    var gateLog = await _gateDomainService.CreateTruckOfConsigneeGateInLog(
                        cntrNo,
                        createGateInDto.PlateNo,
                        dpcInfo.DeliveryPermissionCntrId,
                        $"{dpcInfo.DeliveryPermission.DeliveryPermissionNo}-{cntrNo}.jpg",
                        long.Parse(createGateInDto.GateInNo),
                        createGateInDto.LogType);
                    var newGateInLogInfo = await _gateLogRepository.InsertAsync(gateLog);
                    await _unitOfWork.CommitAsync();

                    try
                    {
                        await PrintingTruckOfConsigneeGateInOperation(newGateInLogInfo);
                        var isTurnedOn = await _lightService.TurningOnConsigneeGateInLight();
                        if (isTurnedOn)
                        {
                            Thread.Sleep(int.Parse(_gateConfiguration.ConsigneeDelayTimeLight));
                            await _lightService.TurningOffConsigneeGateInLight();
                        }
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex);
                        _logger.LogError(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> SavingTruckOfConsigneeGateOutOperation(CreateGateOutDto createGateOutDto)
        {
            GateLog gateInfo=null;
            var result=false;
            Func<GateLog, Task<bool>> PrintingTruckOfConsigneeGateOutOperation = async (GateLog gateLog) =>
            {
                if (gateInfo == null) return false;
                var cntrNo = gateInfo.CntrNoGateIn;
                var cntrInfo = await _containerDomainService.GetContainerInfo(cntrNo);
                var cntrType = await _generalTableDomainService.GetGeneralTablesInfoByTypeAndCode((short)GeneralTypes.ContainerType, cntrInfo.IsoCode.ContainerType);
                await _printerService.PrintTruckOfConsigneeGateOutInfo(new ConsigneeGateOutPrinterRequestDto
                {
                    DeliveryPermissionNo = gateInfo.DeliveryPermissionCntr.DeliveryPermission.DeliveryPermissionNo,
                    CntrNo = gateLog.CntrNoGateIn,
                    Code = gateLog.GateLogNo,
                    GateInTime = gateLog.GateInTime.ToPersianDateTime(),
                    GateOutTime = gateLog.GateOutTime.ToPersianDateTime(),
                    ReferenceNo = gateInfo.DeliveryPermissionCntr.ExitPermitCntrs.First().ExitPermit.ReferenceNo,
                    ExitPermitNo = gateInfo.DeliveryPermissionCntr.ExitPermitCntrs.First().ExitPermit.ExitPermitNo,
                    ExitPermitDate = gateInfo.DeliveryPermissionCntr.ExitPermitCntrs.First().ExitPermit.ExitPermitDate.ToPersianDateTime(),
                    SizeType = $"{cntrInfo.IsoCode.CntrSize} {cntrType.GeneralName}",
                    TruckNo = gateLog.TruckNo
                });
                return true;
            };

            foreach (var CntrNo in createGateOutDto.CntrList)
            {
                try
                {
                    gateInfo = null;
                    gateInfo = await _gateDomainService.GetGateInfoByPlateNoAndCntrNo(createGateOutDto.PlateNo, CntrNo);
                    if (gateInfo != null)
                    {
                        if (gateInfo.CntrNoGateOut != null)
                        {
                            _logger.LogError(Enum.GetName<GateMessagesTemplate>(GateMessagesTemplate.GateOutInfoHasBeenSavedAlready));
                            try
                            {
                                await PrintingTruckOfConsigneeGateOutOperation(gateInfo);
                                
                                var isTurnedOyn = await _lightService.TurningOnConsigneeGateOutLight();
                                if (isTurnedOyn)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(int.Parse(_gateConfiguration.ConsigneeDelayTimeLight)))
                                        .ContinueWith(t => _lightService.TurningOffConsigneeGateOutLight());

                                    //Thread.Sleep(int.Parse(_gateConfiguration.ConsigneeDelayTimeLight));
                                    //await _lightService.TurningOffConsigneeGateOutLight();
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                               // Console.WriteLine(ex);
                                _logger.LogError(ex.Message);
                            }
                            result = false;
                            break;
                        }
                        else if (gateInfo.DeliveryPermissionCntr.ExitPermitCntrs == null)
                        {
                            _logger.LogError(Enum.GetName<GateMessagesTemplate>(GateMessagesTemplate.BijakHasNotBeenIssued));
                            result = false;
                            break;
                        }
                        else
                        {
                            var gateLog = await _gateDomainService.CreateTruckOfConsigneeGateOutLog(
                                gateInfo.GateLogId,
                                $"{gateInfo.DeliveryPermissionCntr.ExitPermitCntrs.FirstOrDefault().ExitPermit.ExitPermitNo}-{gateInfo.CntrNoGateIn}.jpg",
                                long.Parse(createGateOutDto.GateOutNo),
                                createGateOutDto.LogType);
                            await _gateLogRepository.UpdateAsync(gateLog);
                            await _unitOfWork.CommitAsync();
                            //_unitOfWork.Dispose();
                            result = true;
                            try
                            {
                                await PrintingTruckOfConsigneeGateOutOperation(gateLog);
                                var isTurnedOn = await _lightService.TurningOnConsigneeGateOutLight();
                                if (isTurnedOn)
                                {
                                    Thread.Sleep(int.Parse(_gateConfiguration.ConsigneeDelayTimeLight));
                                    await _lightService.TurningOffConsigneeGateOutLight();
                                }
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(ex);
                                _logger.LogError(ex.Message);

                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}