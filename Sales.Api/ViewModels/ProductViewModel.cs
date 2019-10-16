using Sales.Core.Bases;
using Sales.Core.DomainModels.Enums;

namespace Sales.Api.ViewModels
{
    public class ProductViewModel: EntityBase
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Specification { get; set; }
        public ProductUnit ProductUnit { get; set; }
        public int ShelfLife { get; set; }
        public decimal EquivalentTon { get; set; }
        public string Barcode { get; set; }
        public decimal TaxRate { get; set; }
    }
}
