using SpmcoBctsDatabaseMigrator.Infrastructure;
using SpmcoGateAutomation.Domain.Abstracts;
using System.Data;

namespace SpmcoGateAutomation.Infrastructure.EFCore.Abstracts
{
    public class UnitOfWork : IUnitOfWork , IDisposable
    {
        private readonly BctsDatabaseContext _dbContext;
        private bool disposed = false;

        public UnitOfWork(BctsDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CommitAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new DataException();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
    }
}
