using System.Collections.Generic;

namespace Sales.Api.ViewModels.Hateoas
{
    public class LinkedCollectionResourceWrapperViewModel<T> : LinkedResourceBaseViewModel
        where T : LinkedResourceBaseViewModel
    {
        public LinkedCollectionResourceWrapperViewModel(IEnumerable<T> value)
        {
            Value = value;
        }

        public IEnumerable<T> Value { get; set; }
    }
}