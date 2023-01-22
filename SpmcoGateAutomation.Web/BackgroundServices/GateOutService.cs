
using System.ComponentModel;
using System.Drawing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Extensions;
using SpmcoGateAutomation.Common.Configurations;
using SpmcoGateAutomation.Common.Enums;
using SpmcoGateAutomation.Common.Extensions;
using SpmcoGateAutomation.Contract;
using SpmcoGateAutomation.Domain.DomainServices;
using SpmcoGateAutomation.ExternalServices;
using SpmcoGateAutomation.Producer;

namespace SpmcoGateAutomation.Web.BackgroundServices
{
    public class GateOutService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly GateConfiguration _gateConfiguration;
        private readonly IServiceProvider _serviceProvider;
        private FileSystemWatcher watcher;
        private Timer _timer;

        public GateOutService(ILoggerFactory loggerFactory, GateConfiguration gateConfiguration, IServiceProvider serviceProvider)
        {
            this._logger = loggerFactory.CreateLogger<GateOutService>();
            this._gateConfiguration = gateConfiguration;
            this._serviceProvider = serviceProvider;
        }
        public async Task ProcessImages(GateOutImagesDto _gateOutImagesDto)
        {

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                PlateService plateNoService = scope.ServiceProvider.GetRequiredService<PlateService>();
                ContainerService containerService = scope.ServiceProvider.GetRequiredService<ContainerService>();
                IGateApplicationService gateApplicationService = scope.ServiceProvider.GetRequiredService<IGateApplicationService>();
                GateDomainService gateDomainService = scope.ServiceProvider.GetRequiredService<GateDomainService>();
                GateOutLogProducer gateOutLogProducer = scope.ServiceProvider.GetRequiredService<GateOutLogProducer>();
                // Get PlateNo From PlateNo Service

                var resultPlateNo = plateNoService.GetPlateNo(_gateOutImagesDto.PlateImage, out bool hasErrorPlate, false);
                if (hasErrorPlate || string.IsNullOrEmpty(resultPlateNo))
                {
                    //_gateInHubSignalRService
                    //   .SendGateInMessageToClients(GateMessagesTemplate.PlateNoHasNotBeenFound.GetEnumDescription());
                    _logger.LogError(GateMessagesTemplate.PlateNoHasNotBeenFound.GetDisplayName());
                    // When the images can not be processed
                    // we will raise finished event for cleaning directory
                    return;
                }

                // Get ContainerNo From ContainerNo Service
                var OcrContainerDetectList = containerService.GetContainerNo(_gateOutImagesDto);
                if (OcrContainerDetectList == null || OcrContainerDetectList.Count == 0)
                {
                    //_gateInHubSignalRService
                    // .SendGateInMessageToClients(GateMessagesTemplate.PlateNoHasNotBeenFound.GetEnumDescription());
                    _logger.LogError(GateMessagesTemplate.ContainerNoHasNotBeenFound.GetDisplayName());
                    // When the images can not be processed
                    // we will raise finished event for cleaning directory
                    return;
                }

                // Checking Delivery Permission is Ok or not
                var CreateGateOutObj = new CreateGateOutDto();
                CreateGateOutObj.PlateNo = resultPlateNo;
                CreateGateOutObj.LogType = 1;
                CreateGateOutObj.GateOutNo = _gateConfiguration.ConsigneeGateOutNo;
                CreateGateOutObj.CntrList = new List<string>();
                CreateGateOutObj.CntrList.AddRange(OcrContainerDetectList.Where(c => c.CntrNo.Length >= 12).Select(o => o.CntrNo).Distinct().ToList());
                
                var savingResult = await gateApplicationService.SavingTruckOfConsigneeGateOutOperation(CreateGateOutObj);
                // Agar hame chi dorost anjam shode bashe
                // masalan gateout sabt shode ya eenke aslan ghablan sabt shode bood
                // hala migim ke cntrNo ro bargardoon
                if (savingResult)
                {
                    //we will copy images to the location on the server
                    // that has been specialized already
                    foreach (var cntrNo in CreateGateOutObj.CntrList)
                    {
                        var gateInfo = await gateDomainService.GetGateInfoByPlateNoAndCntrNo(resultPlateNo, cntrNo);
                        if (gateInfo != null)
                        {
                            var dpNo = gateInfo.DeliveryPermissionCntr.DeliveryPermission.DeliveryPermissionNo;
                            try
                            {
                                if (Directory.Exists(_gateConfiguration.ConsigneeGateOutSourcePath))
                                {
                                    var DestPath = $"{_gateConfiguration.ConsigneeGateOutDestinationPath}\\{dpNo}\\{cntrNo}";
                                    if (!Directory.Exists(DestPath))
                                    {
                                        Directory.CreateDirectory(DestPath);
                                    }
                                    var properties = typeof(GateOutImagesDto).GetProperties();
                                    //var sideList = new List<string> { "LeftImage", "RightImage", "BackImage" };
                                    foreach (var item in properties)
                                    {
                                        var propertyInfo = _gateOutImagesDto.GetType().GetProperty(item.Name);
                                        var FileName = (propertyInfo.GetValue(_gateOutImagesDto) as ImageInfoDto).ImageName.Split("_").First();
                                        var ItemPath = (propertyInfo.GetValue(_gateOutImagesDto) as ImageInfoDto).Path;
                                        if (!File.Exists($"{DestPath}\\{FileName}.jpg"))
                                        {
                                            File.Copy(ItemPath, $"{DestPath}\\{FileName}.jpg");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                break;
                            }
                            break;
                        }
                    }
                }
            }
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("start");
            if (Directory.Exists(_gateConfiguration.ConsigneeGateOutSourcePath))
            {
                foreach (var item in System.IO.Directory.GetFiles(_gateConfiguration.ConsigneeGateOutSourcePath))
                {
                    System.IO.File.Delete(item);
                }
            }
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(SetFileWatcher, null, TimeSpan.Zero.Seconds, Timeout.Infinite);
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }
        private void SetFileWatcher(object? state)
        {
            _logger.LogInformation("Gate Out");
            //if (!stoppingToken.IsCancellationRequested)
            {
                // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                watcher = new();
                watcher.Path = _gateConfiguration.ConsigneeGateOutSourcePath;

                watcher.IncludeSubdirectories = true;

                watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;

                watcher.Filter = "*.*";


                //Register Event Handler

                watcher.Changed += new FileSystemEventHandler(onChanged);
                watcher.EnableRaisingEvents = true;
            }
            //return Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (Directory.Exists(_gateConfiguration.ConsigneeGateOutSourcePath))
            {
                foreach (var item in System.IO.Directory.GetFiles(_gateConfiguration.ConsigneeGateOutSourcePath))
                {
                    System.IO.File.Delete(item);
                }
            }
            return base.StopAsync(cancellationToken);
        }
        public bool IsPossibleToProcess(GateOutImagesDto gateOutImagesDto)
        {
            return gateOutImagesDto != null &&
                gateOutImagesDto.PlateImage != null && 
                gateOutImagesDto.LeftImage != null && 
                gateOutImagesDto.RightImage != null && 
                gateOutImagesDto.BackImage != null;
        }
        private bool IsDuplicateFile(GateOutImagesDto gateOutImageDto, string propertyName)
        {
            var properties = typeof(GateOutImagesDto).GetProperties();
            var result = properties.Any(c => c.Name == propertyName && (c.GetValue(gateOutImageDto) as ImageInfoDto) != null);
            return result;
        }
        public void onChanged(object source, FileSystemEventArgs e)
        {
            string filename = e.FullPath;
            string sourcePath = _gateConfiguration.ConsigneeGateOutSourcePath;
            DirectoryInfo directorySrcInfo = new DirectoryInfo(sourcePath);
            var files = directorySrcInfo.EnumerateFiles();
            var isDataGathered = files.Select(c => new
            {
                file = c.Name.Split("_").First()
            }).Distinct().Count() == 4;
            if (isDataGathered)
            {
                try
                {
                    using (var _gateOutImageDto = new GateOutImagesDto())
                    { 
                       // Console.WriteLine(e.Name + " is " + e.ChangeType);
                        foreach (var item in files)
                        {
                            if (IsPossibleToProcess(_gateOutImageDto)) break;
                            if (IsDuplicateFile(_gateOutImageDto, item.Name.Split("_")[0] + "Image")) continue;
                            using (var fs = new FileStream(item.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                ImageInfoDto ImageData = new(Image.FromStream(fs), item.FullName, item.Name);
                                switch (item.Name.Split("_")[0])
                                {
                                    case "Plate":
                                        _gateOutImageDto.PlateImage = ImageData;
                                        break;
                                    case "Left":
                                        _gateOutImageDto.LeftImage = ImageData;
                                        break;
                                    case "Right":
                                        _gateOutImageDto.RightImage = ImageData;
                                        break;
                                    case "Back":
                                        _gateOutImageDto.BackImage = ImageData;
                                        break;
                                }
                                ImageData.Dispose();
                                fs.Dispose();
                                fs.Close();
                            }

                        }
                        if (IsPossibleToProcess(_gateOutImageDto))
                        {
                            _ = ProcessImages(_gateOutImageDto).Wait(-1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
                finally
                {
                    //----------------------------------------------------
                    var srcfiles = directorySrcInfo.EnumerateFiles();
                    foreach (FileInfo srcfile in srcfiles)
                    {
                        System.IO.File.Delete(srcfile.FullName);
                    }
                    _logger.LogInformation("delete all image");
                    //Task.Delay(1000).Wait();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    _logger.LogInformation("-----------Waiting For New Truck -----------------");
                }
            }
        }
    }
}
