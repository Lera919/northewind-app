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
    using Bogus;
    using Microsoft.EntityFrameworkCore;
    using Northwind.DataAccess;
    using NorthwindWebApp.Context;
    using NorthwindWebApp.Entities;

    /// <summary>
    /// ProductCategoriesManagmentService.
    /// </summary>
    public class ProductCategoriesManagmentService : IProductsCategoryManagmentService
    {
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoriesManagmentService"/> class.
        /// </summary>
        /// <param name="contex">Northwind contex.</param>
        public ProductCategoriesManagmentService(NorthwindContext contex)
        {
            this.context = contex ?? throw new ArgumentNullException(nameof(contex));
            if (!this.context.Categories.Any())
            {
                this.context.Categories.AddRange(new Faker<Category>("en")
               .RuleFor(x => x.CategoryName, f => f.Commerce.Product())
               .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
               .Generate(15));
                this.context.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(Category productCategory)
        {
            this.context.Categories.Add(productCategory);
            int rowsAffected = await this.context.SaveChangesAsync().ConfigureAwait(false);
            return rowsAffected;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Category> LookupCategoriesByNameAsync(IList<string> names)
        => this.context.Categories.Where(category => names.Contains(category.CategoryName)).OrderBy(category => category.CategoryId).AsAsyncEnumerable();

        /// <inheritdoc/>
        public async Task<(bool result, Category productCategory)> TryGetCategoryAsync(int categoryId)
        {
            var category = await this.context.Categories.FindAsync(categoryId).ConfigureAwait(false);
            return (category != null, category);
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            var (result, productCategory) = await this.TryGetCategoryAsync(categoryId).ConfigureAwait(false);
            if (result)
            {
                this.context.Categories.Remove(productCategory);
                this.context.SaveChanges();
            }

            return result;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Category> GetCategoriesAsync(int offset, int limit)
            => this.context.Categories.OrderBy(category => category.CategoryId).Skip(offset).Take(limit).AsAsyncEnumerable();

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, Category productCategory)
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
                this.context.Categories.Update(productCategory);
                await this.context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
        }
    }
}
