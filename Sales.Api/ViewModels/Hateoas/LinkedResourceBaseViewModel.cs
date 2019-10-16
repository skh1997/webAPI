using System.Collections.Generic;
using Sales.Core.Bases;

namespace Sales.Api.ViewModels.Hateoas
{
    public abstract class LinkedResourceBaseViewModel: EntityBase
    {
        public List<LinkViewModel> Links { get; set; } = new List<LinkViewModel>();
    }
}