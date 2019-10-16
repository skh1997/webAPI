using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sales.Core.DomainModels;
using Sales.Core.DomainModels.Enums;

namespace Sales.Infrastructure.Data
{
    public class SalesContextSeed
    {
        public static async Task SeedAsync(SalesContext salesContext,
                    ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                // TODO: Only run this if using a real database
                // context.Database.Migrate();

                if (!salesContext.Products.Any())
                {
                    salesContext.Products.AddRange(
                        new List<Product>{
                            new Product{
                                Name = "Milk",
                                FullName = "Fresh Milk",
                                Specification = "200ml",
                                ProductUnit = ProductUnit.Bag,
                                ShelfLife = 5,
                                EquivalentTon = new decimal(0.0002),
                                Barcode = "1234567890123",
                                TaxRate = new decimal(0.17)
                            },
                            new Product{
                                Name = "Beer",
                                FullName = "German Beer",
                                Specification = "1000ml",
                                ProductUnit = ProductUnit.Cup,
                                ShelfLife = 180,
                                EquivalentTon = new decimal(0.001),
                                Barcode = "1234567890120",
                                TaxRate = new decimal(0.17)
                            }
                        }
                    );
                    await salesContext.SaveChangesAsync();
                }

                if (!salesContext.Vehicles.Any())
                {
                    salesContext.Vehicles.AddRange(
                        new List<Vehicle>{
                            new Vehicle {
                                Model = "BWM-I5",
                                Owner = "Dave"
                            },
                            new Vehicle {
                                Model = "Benz 200",
                                Owner = "Nick"
                            }
                        });
                    await salesContext.SaveChangesAsync();
                }

                if (!salesContext.Customers.Any())
                {
                    salesContext.Customers.AddRange(
                        new List<Customer> {
                            new Customer {
                                Company = "Microsoft",
                                Name = "Bill Gates",
                                EstablishmentTime = new DateTime(1975, 4, 4)
                            },
                            new Customer {
                                Company = "Google",
                                Name = "Larry Page",
                                EstablishmentTime = new DateTime(1998, 9, 4)
                            }
                        });
                    await salesContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var logger = loggerFactory.CreateLogger<SalesContextSeed>();
                    logger.LogError(ex.Message);
                    await SeedAsync(salesContext, loggerFactory, retryForAvailability);
                }
            }
        }
    }
}