using MassTransit;
using Spmco.Common.Gate;

namespace SpmcoGateAutomation.Producer
{
    public class GateOutLogProducer
    {
        private readonly IPublishEndpoint publishEndpoint;

        public GateOutLogProducer(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Produce(MassTransientGateOutLog data)
        {
            try
            {
                await publishEndpoint.Publish<MassTransientGateOutLog>(data);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}