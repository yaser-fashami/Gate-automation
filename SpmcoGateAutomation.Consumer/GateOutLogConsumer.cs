using MassTransit;
using Spmco.Common.Gate;

namespace SpmcoGateAutomation.Consumer
{
    public class GateOutLogConsumer : IConsumer<MassTransientGateOutLog>
    {
        public Task Consume(ConsumeContext<MassTransientGateOutLog> context)
        {
            // Do Your Work
            throw new NotImplementedException();
        }
    }
}