using MassTransit;
using Spmco.Common.Gate;

namespace SpmcoGateAutomation.Producer
{
    public class GateInLogProducer
    {
        private readonly IPublishEndpoint publishEndpoint;

        public GateInLogProducer(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Produce(MassTransientGateInLog data)
        {
            try
            {
                await publishEndpoint.Publish<MassTransientGateInLog>(data);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}