// <copyright file="ProductManagementService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.Products
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Bogus;
    using Microsoft.EntityFrameworkCore;
    using Northwind.DataAccess;
    using Northwind.DataAccess.Products;
    using NorthwindWebApp.Context;
    using NorthwindWebApp.Entities;

    /// <summary>
    /// Represents a stub for a product management service.
    /// </summary>
    public sealed class ProductManagementService : IProductManagementService
    {
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="context">Northwind context.</param>
        public ProductManagementService(NorthwindContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));

            if (!context.Products.Any())
            {
                context.Products.AddRange(new Faker<Product>("en")
               .RuleFor(x => x.ProductName, f => f.Commerce.Product())
               .Generate(1000));
                context.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            this.context.Products.Add(product);
            int rowsAffected = await this.context.SaveChangesAsync().ConfigureAwait(false);
            return rowsAffected;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId)
        {
            var (result, product) = await this.TryGetProductAsync(productId).ConfigureAwait(false);
            if (result)
            {
                this.context.Products.Remove(product);
                this.context.SaveChanges();
            }

            return result;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
            => this.context.Products.OrderBy(product => product.ProductId).Skip(offset).Take(limit).AsAsyncEnumerable();

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
            => this.context.Products.Where(prod => prod.CategoryId == categoryId).OrderBy(product => product.ProductId).AsAsyncEnumerable();

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> LookupProductsByNameAsync(IList<string> names)
        => this.context.Products.Where(prod => names.Contains(prod.ProductName)).OrderBy(prod => prod.ProductId).AsAsyncEnumerable();

        /// <inheritdoc/>
        public async Task<(bool result, Product product)> TryGetProductAsync(int productId)
        {
            var product = await this.context.Products.FindAsync(productId).ConfigureAwait(false);
            return (product != null, product);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            var productToUpdate = await this.context.Products.FindAsync(productId).ConfigureAwait(false);
            if (productToUpdate is null)
            {
                return false;
            }
            else
            {
                Update(productToUpdate, product);
                this.context.Products.Update(productToUpdate);
                await this.context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
        }

        private static void Update<T>(T updateEntity, T newEntity)
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                property.SetValue(updateEntity, property.GetValue(newEntity));
            }
        }
    }
}
