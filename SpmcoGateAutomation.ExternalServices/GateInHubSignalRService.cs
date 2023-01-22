using Microsoft.AspNetCore.SignalR;

namespace SpmcoGateAutomation.ExternalServices
{
    public class GateInHubSignalRService : Hub
    {
        public GateInHubSignalRService()
        {
        }
        public async Task SendGateInMessageToClients(string message)
        {
            if (Clients != null)
            {
                await Clients.All.SendAsync("getPlateNumber", message);
            }
        }
    }
}
