using Microsoft.EntityFrameworkCore;

namespace SpmcoGateAutomation.Domain.Abstracts
{
    public interface IRepository <Entity> where Entity : class
    {
        DbSet<Entity> DbSet { get; set; }
        Task<Entity> InsertAsync(Entity entity);
        Task UpdateAsync(Entity entity);
        Task DeleteAsync(long id);
        Task DeleteAsync(Entity entity);
        Task<List<Entity>> SelectAsync();
        Task<Entity> FindByIdAsync(long id);
    }
}
