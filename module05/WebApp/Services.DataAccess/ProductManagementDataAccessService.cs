using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;
using NorthwindWebApp.Entities;

namespace Northwind.Services
{
    public class ProductManagementDataAccessService : IProductManagementService
    {

        private readonly IMapper mapper;
        public ProductManagementDataAccessService(NorthwindDataAccessFactory factory, IMapper mapper)
        {
            this.mapper = mapper;
            this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private NorthwindDataAccessFactory Factory { get; set; }

        public async Task<int> CreateProductAsync(Product product) =>
            await this.Factory.GetProductDataAccessObject().InsertProductAsync(this.mapper.Map<ProductTransferObject>(product));

        public async Task<bool> DestroyProductAsync(int productId)
            => await this.Factory.GetProductDataAccessObject().DeleteProductAsync(productId);
        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {

            await foreach (var element in this.Factory.GetProductDataAccessObject().SelectProductsAsync(offset, limit))
            {
                yield return this.mapper.Map<Product>(element);
            }
        }

        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            await foreach (var element in this.Factory.GetProductDataAccessObject().SelectProductByCategoryAsync(new[] { categoryId }))
            {
                yield return this.mapper.Map<Product>(element);
            }
        }

        public async IAsyncEnumerable<Product> LookupProductsByNameAsync(IList<string> names)
        {
            await foreach (var element in this.Factory.GetProductDataAccessObject().SelectProductsByNameAsync(names))
            {
                yield return this.mapper.Map<Product>(element);
            }
        }

        public async Task<(bool result, Product product)> TryGetProductAsync(int productId)
        {
            try
            {
                var product = await this.Factory.GetProductDataAccessObject().FindProductAsync(productId);
                return (true, this.mapper.Map<Product>(product));
            }
            catch (ProductNotFoundException)
            {
                return (false, null);
            }
        }

        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            product.ProductId = productId;
            var result = await this.Factory.GetProductDataAccessObject().UpdateProductAsync(this.mapper.Map<ProductTransferObject>(product));
            return result;
        }
    }
}
