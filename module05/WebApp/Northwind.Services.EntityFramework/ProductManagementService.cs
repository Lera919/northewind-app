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
    using AutoMapper;
    using Bogus;
    using Microsoft.EntityFrameworkCore;
    using Northwind.DataAccess;
    using Northwind.DataAccess.Products;
    using WebAppModule6.Context;
    using WebAppModule6.Entities;

    /// <summary>
    /// Represents a stub for a product management service.
    /// </summary>
    public sealed class ProductManagementService : IProductManagementService
    {
        private readonly NorthwindContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="context">Northwind context.</param>
        public ProductManagementService(NorthwindContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            this.context.Products.Add(this.mapper.Map<ProductEntity>(product));
            int rowsAffected = await this.context.SaveChangesAsync().ConfigureAwait(false);
            return rowsAffected;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId)
        {
            var (result, product) = await this.TryGetProductAsync(productId).ConfigureAwait(false);
            if (result)
            {
                this.context.Products.Remove(this.mapper.Map<ProductEntity>(product));
                this.context.SaveChanges();
            }

            return result;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            await foreach (var product in this.context.Products.Include(a => a.Category).OrderBy(product => product.ProductId).Skip(offset).Take(limit).AsAsyncEnumerable())
            {
                yield return this.mapper.Map<Product>(product);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            await foreach(var product in this.context.Products.Include(a => a.Category).Where(prod => prod.CategoryId == categoryId).OrderBy(product => product.ProductId).AsAsyncEnumerable())
            {
                yield return this.mapper.Map<Product>(product);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> LookupProductsByNameAsync(IList<string> names)
        {
            await foreach (var product in this.context.Products.Include(a => a.Category).Where(prod => names.Contains(prod.ProductName)).OrderBy(prod => prod.ProductId).AsAsyncEnumerable())
            {
                yield return this.mapper.Map<Product>(product);
            }
        }

        /// <inheritdoc/>
        public async Task<(bool result, Product product)> TryGetProductAsync(int productId)
        {
            var product = await this.context.Products.FindAsync(productId).ConfigureAwait(false);
            return (product != null, this.mapper.Map<Product>(product));
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
                Update(this.mapper.Map<ProductEntity>(productToUpdate), this.mapper.Map<ProductEntity>(product));
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
