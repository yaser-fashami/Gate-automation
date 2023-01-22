using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Spmco.Common.Gate;
using SpmcoGateAutomation.Common.Extensions;
using SpmcoGateAutomation.ExternalServices;

namespace SpmcoGateAutomation.Consumer
{
    public class GateInLogConsumer : IConsumer<MassTransientGateInLog>
    {
        private readonly IHubContext<GateInHubSignalRService> hub;

        public GateInLogConsumer(IHubContext<GateInHubSignalRService> hub)
        {
            this.hub = hub;
        }
        public async Task Consume(ConsumeContext<MassTransientGateInLog> context)
        {
            // Do Your Work
            var data = context.Message;
            if (hub.Clients != null)
            {
                //await hub.Clients.All.SendAsync("GateInReceiver", data);
            }
            throw new NotImplementedException();
        }
    }
}