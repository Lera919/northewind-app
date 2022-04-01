using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Northwind.DataAccess;
using Northwind.Services.Products;
using WebAppModule6.Context;
using WebAppModule6.Entities;

namespace WebAppModule6
{
    public class MemoryDataCreator
    {
        public MemoryDataCreator(NorthwindContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(new Faker<CategoryEntity>("en")
               .RuleFor(x => x.CategoryName, f => f.Commerce.Product())
               .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
               .Generate(1000)
                   );
                context.SaveChanges();
            }
            if (!context.Products.Any())
            {
                context.Products.AddRange(new Faker<ProductEntity>("en")
               .RuleFor(x => x.ProductName, f => f.Commerce.Product())
               .Generate(1000)
                   );
                context.SaveChanges();
            }
        }


    }
}
