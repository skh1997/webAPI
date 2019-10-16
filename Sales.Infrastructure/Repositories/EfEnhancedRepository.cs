using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sales.Core.Bases;
using Sales.Infrastructure.Data;
using Sales.Infrastructure.Extensions;
using Sales.Infrastructure.Interfaces;
using Sales.Infrastructure.UsefulModels.Pagination;

namespace Sales.Infrastructure.Repositories
{
    public class EfEnhancedRepository<T> : EfRepository<T>, IEnhancedRepository<T> where T : EntityBase
    {
        public EfEnhancedRepository(SalesContext context): base(context)
        {
        }

        public async Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, PropertyMapping propertyMapping)
        {
            var collectionBeforePaging = Context.Set<T>().ApplySort(parameters.OrderBy, propertyMapping);
            parameters.Count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<T>(parameters, items);
            return result;
        }

        public async Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, PropertyMapping propertyMapping, Expression<Func<T, bool>> criteria) 
        {
            var collectionBeforePaging = Context.Set<T>().Where(criteria).ApplySort(parameters.OrderBy, propertyMapping);
            parameters.Count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<T>(parameters, items);
            return result;
        }

        public async Task<PaginatedList<T>> GetPaginatedAsync(PaginationBase parameters, PropertyMapping propertyMapping, Expression<Func<T, bool>> criteria, 
            params Expression<Func<T, object>>[] includes)
        {
            var collectionBeforePaging = includes
                .Aggregate(Context.Set<T>().Where(criteria).ApplySort(parameters.OrderBy, propertyMapping),
                    (current, include) => current.Include(include));
            parameters.Count = await collectionBeforePaging.CountAsync();
            var items = await collectionBeforePaging.Skip(parameters.PageIndex * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var result = new PaginatedList<T>(parameters, items);
            return result;
        }
    }
}
