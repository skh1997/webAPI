using System.Threading;
using System.Threading.Tasks;
using Sales.Core.Interfaces;

namespace Sales.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SalesContext _context;

        public UnitOfWork(SalesContext context)
        {
            _context = context;
        }
        
        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }

        public bool Save(bool acceptAllChangesOnSuccess)
        {
            return _context.SaveChanges(acceptAllChangesOnSuccess) >= 0;
        }

        public async Task<bool> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken) >= 0;
        }

        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _context.SaveChangesAsync(cancellationToken) >= 0;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
