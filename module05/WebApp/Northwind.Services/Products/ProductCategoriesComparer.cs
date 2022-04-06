using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services.Products
{
    public class ProductCategoriesComparer : IEqualityComparer<ProductCategory>
    {

        public bool Equals(ProductCategory x, ProductCategory y)
        {
            return x.CategoryName.Equals(y.CategoryName);
        }

        public int GetHashCode(ProductCategory obj)
        {
            return obj.CategoryName.GetHashCode();
        }

    }
}
