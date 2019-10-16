using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Sales.Core.Interfaces;

namespace Sales.Infrastructure.Interfaces
{
    public interface ICoreService<out T> : IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        ILogger<T> Logger { get; }
        IMapper Mapper { get; }
    }
}