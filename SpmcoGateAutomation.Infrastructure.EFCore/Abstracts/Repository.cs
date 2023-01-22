using Microsoft.EntityFrameworkCore;
using SpmcoBctsDatabaseMigrator.Infrastructure;
using SpmcoGateAutomation.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmcoGateAutomation.Infrastructure.EFCore.Abstracts
{
    public class Repository<Entity> : IRepository<Entity> where Entity : class
    {
        internal BctsDatabaseContext Context;
        public virtual DbSet<Entity> DbSet { get; set; }
        public Repository(BctsDatabaseContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<Entity>();
        }
        public virtual async Task<Entity> InsertAsync(Entity entity)
        {

                return DbSet.Add(entity).Entity;
        }

        public virtual async Task UpdateAsync(Entity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task DeleteAsync(long id)
        {
            var entityToDelete = DbSet.Find(id);
            DbSet.Remove(entityToDelete);
        }

        public virtual async Task DeleteAsync(Entity entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
        }

        public virtual async Task<List<Entity>> SelectAsync()
        {
            var result = DbSet.AsNoTracking().ToListAsync();
            return await result;
        }

        public virtual async Task<Entity> FindByIdAsync(long id)
        {
            var result = DbSet.FindAsync(id);
            return await result;
        }
    }
}
