
using System.Drawing;
using SpmcoGateAutomation.Contract;
using SpmcoGateAutomation.Common.Configurations;
using SpmcoGateAutomation.ExternalServices;
using SpmcoGateAutomation.Domain.DomainServices;
using SpmcoGateAutomation.Producer;

namespace SpmcoGateAutomation.Web.BackgroundServices
{
    public class GateInService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly GateConfiguration _gateConfiguration;
        private readonly IServiceProvider _serviceProvider;
        public event EventHandler AfterAdd;
        private FileSystemWatcher _watcher;
        private Timer _timer;

        public GateInService(ILoggerFactory loggerFactory, GateConfiguration gateConfiguration, IServiceProvider serviceProvider)
        {
            this._logger = loggerFactory.CreateLogger<GateInService>();
            this._gateConfiguration = gateConfiguration;
            this._serviceProvider = serviceProvider;
        }
        public async Task ProcessImagesAsync(GateInImageDto _gateInImageDto)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                PlateService plateNoService = scope.ServiceProvider.GetRequiredService<PlateService>();
                IGateApplicationService gateApplicationService = scope.ServiceProvider.GetRequiredService<IGateApplicationService>();
                GateDomainService gateDomainService = scope.ServiceProvider.GetRequiredService<GateDomainService>();
                GateInLogProducer gateInLogProducer = scope.ServiceProvider.GetRequiredService<GateInLogProducer>();

                bool hasError = false;
                var resultPlateNo = plateNoService.GetPlateNo(_gateInImageDto.PlateImage, out hasError, true);
                if (hasError)
                {
                    //_gateInHubSignalRService
                    //   .SendGateInMessageToClients(GateMessagesTemplate.PlateNoHasNotBeenFound.GetEnumDescription());
                    _logger.LogError(resultPlateNo);
                    // When the images can not be processed
                    // we will raise finished event for cleaning directory
                    return;
                }

                await gateInLogProducer.Produce(new Spmco.Common.Gate.MassTransientGateInLog
                {
                    PlateNo = "123qwe123"
                });

                var savingResult = await gateApplicationService.SavingTruckOfConsigneeGateInOperation(
                    new CreateGateInDto { PlateNo = resultPlateNo, LogType = 1, GateInNo = _gateConfiguration.ConsigneeGateInNo });

                if (savingResult)
                {
                    var dpcInfo = await gateDomainService.GetDeliveryPermissionCntrInfoByPlateNo(resultPlateNo);
                    if (dpcInfo != null)
                    {
                        var cntrNo = dpcInfo.ReceiptCntrConsignee.Blcntr.ManifestCntr.CntrNo;
                        var deliveryPermissionNo = dpcInfo.DeliveryPermission.DeliveryPermissionNo;
                        if (Directory.Exists(_gateConfiguration.ConsigneeGateInSourcePath))
                        {
                            if (!Directory.Exists(_gateConfiguration.ConsigneeGateInDestinationPath))
                            {
                                Directory.CreateDirectory(_gateConfiguration.ConsigneeGateInDestinationPath);
                            }
                            if (!File.Exists($"{_gateConfiguration.ConsigneeGateInDestinationPath}" +
                                    $"\\{deliveryPermissionNo}" +
                                    $"-{cntrNo}" +
                                    $"{_gateInImageDto.PlateImage.ImageName}.jpg") && File.Exists(_gateInImageDto.PlateImage.Path))
                            {
                                File.Copy(_gateInImageDto.PlateImage.Path,
                                    $"{_gateConfiguration.ConsigneeGateInDestinationPath}" +
                                    $"\\{deliveryPermissionNo}" +
                                    $"-{cntrNo}-" +
                                    $"{_gateInImageDto.PlateImage.ImageName}.jpg");
                            }
                        }
                    }
                }
            }
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            string sourcePath = _gateConfiguration.ConsigneeGateInSourcePath;
            DirectoryInfo directorySrcInfo = new DirectoryInfo(sourcePath);
            FileInfo[] srcfiles = directorySrcInfo.GetFiles();
            foreach (FileInfo srcfile in srcfiles)
            {
                System.IO.File.Delete(srcfile.FullName);
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
            //await Task.Delay(1000, stoppingToken);

            _watcher = new();
            _watcher.Path = _gateConfiguration.ConsigneeGateInSourcePath;

            _watcher.IncludeSubdirectories = true;

            _watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;

            _watcher.Filter = "*.*";


            //Register Event Handler

            _watcher.Created += new FileSystemEventHandler(onCreated);
            _watcher.Changed += new FileSystemEventHandler(onChanged);
            _watcher.Deleted += new FileSystemEventHandler(onDeleted);
            _watcher.EnableRaisingEvents = true;
        }

        private void onDeleted(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine("del " + e.FullPath);
        }
        private void onCreated(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine("create " + e.FullPath);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (Directory.Exists(_gateConfiguration.ConsigneeGateInSourcePath))
            {
                foreach (var item in System.IO.Directory.GetFiles(_gateConfiguration.ConsigneeGateInSourcePath))
                {
                    System.IO.File.Delete(item);
                }
            }
            return base.StopAsync(cancellationToken);
        }
        public void onChanged(object source, FileSystemEventArgs e)
        {
            string filename = e.FullPath;
            string sourcePath = _gateConfiguration.ConsigneeGateInSourcePath;
            DirectoryInfo directorySrcInfo = new DirectoryInfo(sourcePath);
            var files = directorySrcInfo.EnumerateFiles();
            var createdFile = files.Where(c => String.Compare(c.FullName, filename) == 0).FirstOrDefault();
            if (createdFile != null)
            {
                try
                {
                    using (var _gateInImageDto = new GateInImageDto())
                    {
                        //Console.WriteLine(e.Name + " is " + e.ChangeType);
                        _logger.LogInformation(e.Name + " is " + e.ChangeType);
                        using (var fs = new FileStream(e.FullPath.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            ImageInfoDto ImageData = new(Image.FromStream(fs), e.FullPath.ToString(), e.Name);
                            _logger.LogInformation("plate image");

                            _gateInImageDto.PlateImage = ImageData;
                            fs.Close();

                            _ = ProcessImagesAsync(_gateInImageDto).Wait(-1);
                            ImageData.Dispose();

                            fs.Dispose();
                            fs.Close();
                        };

                    };
                }
                catch (Exception ex)
                {

                    //Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                }
                finally
                {
                    //----------------------------------------------------
                    try
                    {
                        string srcPath = _gateConfiguration.ConsigneeGateInSourcePath;
                        DirectoryInfo dirSrcInfo = new DirectoryInfo(srcPath);
                        var srcfiles = dirSrcInfo.EnumerateFiles();
                        foreach (FileInfo srcfile in srcfiles)
                        {
                            System.IO.File.Delete(srcfile.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                    //----------------------------------------------------
                    //if (Directory.Exists(_gateConfiguration.ConsigneeGateInSourcePath))
                    //{
                    //foreach (var item in System.IO.Directory.GetFiles(_gateConfiguration.ConsigneeGateInSourcePath))
                    //{
                    //    try
                    //    {
                    //        System.IO.File.Delete(item);
                    //    }
                    //    catch (Exception ex)
                    //    {

                    //        _logger.LogError("delete file",ex.Message);
                    //    }
                    //}
                    //}
                    //_logger.LogInformation("Delete PlateNo");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    _logger.LogInformation("-----------Waiting For New Truck -----------------");
                }
            }
        }
    }
}
