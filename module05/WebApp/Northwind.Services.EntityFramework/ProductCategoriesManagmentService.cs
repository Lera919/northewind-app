// <copyright file="ProductCategoriesManagmentService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.Products
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Bogus;
    using Microsoft.EntityFrameworkCore;
    using Northwind.DataAccess;
    using WebAppModule6.Context;
    using WebAppModule6.Entities;

    /// <summary>
    /// ProductCategoriesManagmentService.
    /// </summary>
    public class ProductCategoriesManagmentService : IProductsCategoryManagmentService
    {
        private readonly NorthwindContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoriesManagmentService"/> class.
        /// </summary>
        /// <param name="contex">Northwind contex.</param>
        public ProductCategoriesManagmentService(NorthwindContext contex, IMapper mapper)
        {
            this.context = contex ?? throw new ArgumentNullException(nameof(contex));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            this.context.Categories.Add(this.mapper.Map<CategoryEntity>(productCategory));
            int rowsAffected = await this.context.SaveChangesAsync().ConfigureAwait(false);
            return rowsAffected;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategory> LookupCategoriesByNameAsync(IList<string> names)
        {
            await foreach (var category in this.context.Categories.Where(category => names.Contains(category.CategoryName)).OrderBy(category => category.CategoryId).AsAsyncEnumerable())
            {
                yield return this.mapper.Map<ProductCategory>(category);
            }
        }

        /// <inheritdoc/>
        public async Task<(bool result, ProductCategory productCategory)> TryGetCategoryAsync(int categoryId)
        {
            var category = await this.context.Categories.FindAsync(categoryId).ConfigureAwait(false);
            return (category != null, this.mapper.Map<ProductCategory>(category));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            var (result, productCategory) = await this.TryGetCategoryAsync(categoryId).ConfigureAwait(false);
            if (result)
            {
                this.context.Categories.Remove(this.mapper.Map<CategoryEntity>(productCategory));
                this.context.SaveChanges();
            }

            return result;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            await foreach (var category in this.context.Categories.OrderBy(category => category.CategoryId).Skip(offset).Take(limit).AsAsyncEnumerable())
            {
                yield return this.mapper.Map<ProductCategory>(category);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            if (categoryId != productCategory.CategoryId)
            {
                return false;
            }

            var categoryToUpdate = await this.context.Categories.FindAsync(categoryId).ConfigureAwait(false);
            if (categoryToUpdate is null)
            {
                return false;
            }
            else
            {
                this.context.Categories.Update(this.mapper.Map<CategoryEntity>(productCategory));
                await this.context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
        }

        public async IAsyncEnumerable<ProductCategory> GetAllCategoriesAsync()
        {
            await foreach (var category in this.context.Categories.OrderBy(x => x.CategoryId).AsAsyncEnumerable())
            {
                yield return this.mapper.Map<ProductCategory>(category);
            }
        }
    }
}
