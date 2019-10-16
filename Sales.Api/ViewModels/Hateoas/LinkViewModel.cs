namespace Sales.Api.ViewModels.Hateoas
{
    public class LinkViewModel
    {
        public LinkViewModel(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
        
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }
}