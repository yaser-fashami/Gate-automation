
using SpmcoGateAutomation.Contract;
using cm;
using gx;
using SpmcoGateAutomation.Common.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace SpmcoGateAutomation.ExternalServices
{
    public class PlateService
    {
        private readonly IHubContext<GateInHubSignalRService> _gateInHubContext;
        private readonly IHubContext<GateOutHubSignalRService> _gateOutHubContext;

        public PlateService(IHubContext<GateInHubSignalRService> gateInHubContext, IHubContext<GateOutHubSignalRService> gateOutHubContext)
        {
            _gateInHubContext = gateInHubContext;
            _gateOutHubContext = gateOutHubContext;
        }

        public string GetPlateNo(ImageInfoDto plateImage, out bool hasError, bool fromGateIn)
        {
            try
            {
                using (cmAnpr anpr = new cmAnpr("default"))
                {

                    using (gxImage image = new gxImage("default"))
                    {
                        if (!anpr.CheckLicenses4Engine("", 0))
                        {
                            hasError = true;
                            return "Cannot find licenses for the current engine!!!";
                        }
                        image.Load(plateImage.Path);

                        if (anpr.FindFirst(image))
                        {
                            var plateRecognition = anpr.GetText();
                            hasError = false;
                            var carPlateSection1 = plateRecognition.Substring(0, 2);
                            var carPlateSection2 = plateRecognition.Substring(2, 1);
                            var carPlateSection3 = plateRecognition.Substring(3, 3);
                            var carPlateSection4 = plateRecognition.Substring(6, 2);
                            if (fromGateIn)
                            {
                                _gateInHubContext.Clients.All.SendAsync("getPlateNumber", new CarPlateNumberDto()
                                {
                                    CarPlateSection1 = carPlateSection1,
                                    CarPlateSection2 = carPlateSection2,
                                    CarPlateSection3 = carPlateSection3,
                                    CarPlateSection4 = carPlateSection4
                                });
                            }
                            else
                            {
                                _gateOutHubContext.Clients.All.SendAsync("getPlateNumber", new CarPlateNumberDto()
                                {
                                    CarPlateSection1 = carPlateSection1,
                                    CarPlateSection2 = carPlateSection2,
                                    CarPlateSection3 = carPlateSection3,
                                    CarPlateSection4 = carPlateSection4
                                });
                            }
                            return plateRecognition.ToEnglishNumber();
                        }
                        hasError = true;
                        //Console.WriteLine("Finish plate image process");
                        return "Plate No was not found";
                    };
                };
            }
            catch (gxException e)
            {
                hasError = true;
                //Console.WriteLine("error Finish plate image process");
                return $"Exception occurred: {e.Message}";
            }
        }
    }

    public class CarPlateNumberDto
    {
        public string CarPlateSection1 { get; set; }
        public string CarPlateSection2 { get; set; }
        public string CarPlateSection3 { get; set; }
        public string CarPlateSection4 { get; set; }
    }
}
