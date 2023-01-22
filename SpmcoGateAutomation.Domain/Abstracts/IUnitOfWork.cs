using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace SpmcoGateAutomation.Domain.Abstracts
{
    [ScopedService]
    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Dispose();
    }
}
