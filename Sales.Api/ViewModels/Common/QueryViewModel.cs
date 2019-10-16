using Sales.Infrastructure.UsefulModels.Pagination;

namespace Sales.Api.ViewModels.Common
{
    public class QueryViewModel : PaginationBase
    {
        public string SearchTerm { get; set; }
        public string Fields { get; set; }
    }
}
