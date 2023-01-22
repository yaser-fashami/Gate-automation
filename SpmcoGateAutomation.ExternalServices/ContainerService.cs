using cm;
using gx;
using Microsoft.AspNetCore.SignalR;
using SpmcoGateAutomation.Common.CommonDto;
using SpmcoGateAutomation.Contract;

namespace SpmcoGateAutomation.ExternalServices
{
    public class ContainerService
    {
        private IHubContext<GateOutHubSignalRService> _gateOutHubContext;

        public ContainerService(IHubContext<GateOutHubSignalRService> gateOutHubContext)
        {
            _gateOutHubContext = gateOutHubContext;
        }

        public List<OcrContainerDetectDto> GetContainerNo(GateOutImagesDto gateOutImagesDto)
        {
            var properties = typeof(GateOutImagesDto).GetProperties();
            var sideList = new List<string> { "LeftImage", "RightImage", "BackImage" };
            List<OcrContainerDetectDto> OcrResult = new List<OcrContainerDetectDto>();
            foreach (var item in properties)
            {
                try
                {
                    using (var accr = new cmAccr())
                    {
                        accr.Reset();
                        var propertyInfo = gateOutImagesDto.GetType().GetProperty(item.Name);
                        if (sideList.Contains(item.Name))
                        {
                            using (var image = new gxImage("default"))
                            {
                                image.Load((propertyInfo.GetValue(gateOutImagesDto) as ImageInfoDto).Path);
                                accr.AddImage(image, 0);
                            }
                            accr.FindFirstContainerCode();
                            var info = ExtractData(accr);
                            if (info != null)
                            {
                                OcrResult.Add(info);
                            }
                        }
                    }
                }
                catch (gxException ex)
                {
                    throw ex;
                }
            }
            string containerNumber = OcrResult[0].CntrNo;
            _gateOutHubContext.Clients.All.SendAsync("containerNumber", containerNumber);
            return OcrResult;
        }
        private OcrContainerDetectDto ExtractData(cmAccr ocr)
        {
            var code = ocr.GetCode(); try
            {
                return new OcrContainerDetectDto
                {
                    CntrNo = code.Substring(0, 4) + " " + code.Substring(4, 7),
                    IsoCode = code.Substring(code.Length - 4, 4),
                    CheckSum = byte.Parse(code.Substring(code.Length - 5, 1)),
                    CalculatedCheckSum = (byte)ocr.GetChecksum(),
                    //IsValid = ocr.IsValid() && ocr.ChecksumIsValid() > 0
                    IsValid = ocr.IsValid()
                };
            }
            catch { return null; }
        }
    }
}
