using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Northwind.DataAccess;
using Northwind.Services.Products;
using NorthwindWebApp.Context;
using NorthwindWebApp.Entities;

namespace NorthwindWebApp
{
    public class MemoryDataCreator
    {
        private readonly NorthwindContext context;

        public MemoryDataCreator(NorthwindContext context)
        {
            this.context = context;
        }

        public async Task SeedDate()
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(new Faker<CategoryEntity>("en")
               .RuleFor(x => x.CategoryName, f => f.Commerce.Product())
               .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
               .Generate(100)
                   );
                context.SaveChanges();
            }
            if (!context.Products.Any())
            {
                context.Products.AddRange(new Faker<ProductEntity>("en")
               .RuleFor(x => x.ProductName, f => f.Commerce.Product())
               .Generate(100)
                   );
                context.SaveChanges();
            }
            if (context.Employees.Count() < 10)
            {
                context.Employees.AddRange(new Faker<EmployeeEntity>("en")
               .RuleFor(x => x.LastName, f => f.Person.LastName)
                .RuleFor(x => x.FirstName, f => f.Person.FirstName)
                 //.RuleFor(x => x.Photo, f => (new HttpClient().GetByteArrayAsync(f.Internet.Avatar()).Result))
                   .Generate(100));
               
                context.SaveChanges();
            }
        }


    }
}
