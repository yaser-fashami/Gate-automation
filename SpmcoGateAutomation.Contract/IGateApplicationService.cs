using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SpmcoGateAutomation.Contract
{
    [ScopedService]
    public interface IGateApplicationService
    {
        Task<bool> SavingTruckOfConsigneeGateInOperation(CreateGateInDto createGateInDto);
        Task<bool> SavingTruckOfConsigneeGateOutOperation(CreateGateOutDto createGateOutDto);
    }
}