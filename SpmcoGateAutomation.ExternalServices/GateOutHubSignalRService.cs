using Microsoft.AspNetCore.SignalR;


namespace SpmcoGateAutomation.ExternalServices
{
    public class GateOutHubSignalRService : Hub
    {
        public GateOutHubSignalRService()
        {
        }
        public async Task SendMessageToClients(string message)
        {
            if (Clients != null)
            {
            }
        }
    }
}
