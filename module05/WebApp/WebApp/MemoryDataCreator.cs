using System;
using System.Collections.Generic;
using System.Linq;
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
        public MemoryDataCreator(NorthwindContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(new Faker<Category>("en")
               .RuleFor(x => x.CategoryName, f => f.Commerce.Product())
               .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
               .Generate(1000)
                   );
                context.SaveChanges();
            }
            if (!context.Products.Any())
            {
                context.Products.AddRange(new Faker<Product>("en")
               .RuleFor(x => x.ProductName, f => f.Commerce.Product())
               .Generate(1000)
                   );
                context.SaveChanges();
            }
        }


    }
}
