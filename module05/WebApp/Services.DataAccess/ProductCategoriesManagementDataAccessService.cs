using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;
using NorthwindWebApp.Entities;

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

        public async Task<int> CreateCategoryAsync(Category productCategory)
       =>
            await this.Factory.GetProductCategoryDataAccessObject().InsertProductCategoryAsync(this.mapper.Map<ProductCategoryTransferObject>(productCategory));

        public async Task<bool> DestroyCategoryAsync(int categoryId)
            => await this.Factory.GetProductCategoryDataAccessObject().DeleteProductCategoryAsync(categoryId);


        public async IAsyncEnumerable<Category> GetCategoriesAsync(int offset, int limit)
        {

            await foreach (var element in this.Factory.GetProductCategoryDataAccessObject().SelectProductCategoriesAsync(offset, limit))
            {
                yield return this.mapper.Map<Category>(element);
            }
        }

        public async IAsyncEnumerable<Category> LookupCategoriesByNameAsync(IList<string> names)
        {
            await foreach (var element in this.Factory.GetProductCategoryDataAccessObject().SelectProductCategoriesByNameAsync(names))
            {
                yield return this.mapper.Map<Category>(element);
            }
        }

        public async Task<(bool result, Category productCategory)> TryGetCategoryAsync(int categoryId)
        {
            var category = await this.Factory.GetProductCategoryDataAccessObject().FindProductCategoryAsync(categoryId);
            var result = category is null ? null : this.mapper.Map<Category>(category);
            return (result is not null, result);
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, Category productCategory)
        {
            productCategory.CategoryId = categoryId;
            return await this.Factory.GetProductCategoryDataAccessObject().UpdateProductCategoryAsync(this.mapper.Map<ProductCategoryTransferObject>(productCategory));
        }
    }
}
