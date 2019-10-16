using System;
using Sales.Core.Bases;

namespace Sales.Core.DomainModels
{
    public class Customer: EntityBase
    {
        public string Company { get; set; }
        public string Name { get; set; }
        public DateTimeOffset EstablishmentTime { get; set; }
    }
}