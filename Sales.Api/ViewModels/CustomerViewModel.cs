using System;
using Sales.Core.Bases;

namespace Sales.Api.ViewModels
{
    public class CustomerViewModel: EntityBase
    {
        public string Company { get; set; }
        public string Name { get; set; }
        public DateTimeOffset EstablishmentTime { get; set; }
    }
}