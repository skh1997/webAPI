using Sales.Core.Bases;

namespace Sales.Core.DomainModels
{
    public class Vehicle: EntityBase
    {
        public string Model { get; set; }
        public string Owner { get; set; }
    }
}