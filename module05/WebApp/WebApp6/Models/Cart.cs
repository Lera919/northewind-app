using Northwind.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp6.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        public void AddItem(Product product, int quantity)
        {
            CartLine viewModel = Lines.FirstOrDefault(p => p.Product.ProductId == product.ProductId);

            if (viewModel == null)
            {
                Lines.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                viewModel.Quantity += quantity;
            }
        }

        public void RemoveLine(Product product) =>
            Lines.RemoveAll(l => l.Product.ProductId == product.ProductId);

        public decimal ComputeTotalValue() =>
            Lines.Sum(e => e.Product.UnitPrice.Value * e.Quantity);

        public void Clear() => Lines.Clear();
    }
}
