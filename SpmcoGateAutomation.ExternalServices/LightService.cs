using System.Net;
using System.Net.Http;

namespace SpmcoGateAutomation.ExternalServices
{
    public class LightService
    {
        private readonly HttpClient _consigneeGateInLightApiHttpClient;
        private readonly HttpClient _consigneeGateOutLightApiHttpClient;

        public LightService(IHttpClientFactory clientFactory)
        {
            this._consigneeGateInLightApiHttpClient = clientFactory.CreateClient("ConsigneeGateInLightApiUri");
            this._consigneeGateOutLightApiHttpClient = clientFactory.CreateClient("ConsigneeGateOutLightApiUri");
            _consigneeGateInLightApiHttpClient.Timeout = TimeSpan.FromSeconds(2);
            _consigneeGateOutLightApiHttpClient.Timeout = TimeSpan.FromSeconds(2);
        }
        public async Task<bool> TurningOnConsigneeGateInLight()
        {
            try
            {
                _consigneeGateInLightApiHttpClient.CancelPendingRequests();
                var result =  _consigneeGateInLightApiHttpClient.GetAsync("/trigger/gpiotrigger?gpout=1&wfilter=1").Result;
                if (result != null && result.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("on light" + ex.Message + "s" + ex.StackTrace);
                return false;
            }
        }
        public async Task<bool> TurningOffConsigneeGateInLight()
        {
            try
            {
                var result =  _consigneeGateInLightApiHttpClient.GetAsync("/trigger/gpiotrigger?gpout=0&wfilter=1").Result;
                if (result != null && result.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("off light " + ex.Message + "s" + ex.StackTrace);
                return false;
            }
        }
        public async Task<bool> TurningOnConsigneeGateOutLight()
        {
            try
            {
                _consigneeGateOutLightApiHttpClient.CancelPendingRequests();
                var result =  _consigneeGateOutLightApiHttpClient.GetAsync("/trigger/gpiotrigger?gpout=1&wfilter=1").Result;
                if (result != null && result.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch (AggregateException ex)
            {
                return false;
            }
            return false;
        }
        public async Task<bool> TurningOffConsigneeGateOutLight()
        {
            try
            {
                var result = _consigneeGateOutLightApiHttpClient.GetAsync("/trigger/gpiotrigger?gpout=0&wfilter=1").Result;
                if (result != null && result.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                } 
            }
            catch (AggregateException ex)
            {

                return false;
            }
            return false;
        }
    }
}
