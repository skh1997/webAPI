using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sales.Core.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        bool Save();
        bool Save(bool acceptAllChangesOnSuccess);
        Task<bool> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}