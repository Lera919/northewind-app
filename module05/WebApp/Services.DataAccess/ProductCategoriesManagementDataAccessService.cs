using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;
using WebAppModule6.Entities;

namespace Northwind.Services.DataAccess
{
    public class ProductCategoriesManagementDataAccessService : IProductsCategoryManagmentService
    {
        private readonly IMapper mapper;
        public ProductCategoriesManagementDataAccessService(NorthwindDataAccessFactory factory, IMapper mapper)
        {
            this.mapper = mapper;
            this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private NorthwindDataAccessFactory Factory { get; set; }

        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
       =>
            await this.Factory.GetProductCategoryDataAccessObject().InsertProductCategoryAsync(this.mapper.Map<ProductCategoryTransferObject>(productCategory));

        public async Task<bool> DestroyCategoryAsync(int categoryId)
            => await this.Factory.GetProductCategoryDataAccessObject().DeleteProductCategoryAsync(categoryId);

        public async IAsyncEnumerable<ProductCategory> GetAllCategoriesAsync()
        {
            await foreach (var element in this.Factory.GetProductCategoryDataAccessObject().SelectProductCategoriesAsync())
            {
                yield return this.mapper.Map<ProductCategory>(element);
            }
        }

        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {

            await foreach (var element in this.Factory.GetProductCategoryDataAccessObject().SelectProductCategoriesAsync(offset, limit))
            {
                yield return this.mapper.Map<ProductCategory>(element);
            }
        }

        public async IAsyncEnumerable<ProductCategory> LookupCategoriesByNameAsync(IList<string> names)
        {
            await foreach (var element in this.Factory.GetProductCategoryDataAccessObject().SelectProductCategoriesByNameAsync(names))
            {
                yield return this.mapper.Map<ProductCategory>(element);
            }
        }

        public async Task<(bool result, ProductCategory productCategory)> TryGetCategoryAsync(int categoryId)
        {
            var category = await this.Factory.GetProductCategoryDataAccessObject().FindProductCategoryAsync(categoryId);
            var result = category is null ? null : this.mapper.Map<ProductCategory>(category);
            return (result is not null, result);
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            productCategory.CategoryId = categoryId;
            return await this.Factory.GetProductCategoryDataAccessObject().UpdateProductCategoryAsync(this.mapper.Map<ProductCategoryTransferObject>(productCategory));
        }
    }
}
