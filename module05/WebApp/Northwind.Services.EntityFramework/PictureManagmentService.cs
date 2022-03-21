// <copyright file="PictureManagmentService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.Products
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Bogus;
    using Northwind.DataAccess;
    using NorthwindWebApp.Context;
    using NorthwindWebApp.Entities;

    /// <summary>
    /// PictureManagmentService.
    /// </summary>
    public class PictureManagmentService : IProductPictureManagementService
    {
        private const int ReservedBytes = 78;
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureManagmentService"/> class.
        /// </summary>
        /// <param name="context">Northwind context.</param>
        public PictureManagmentService(NorthwindContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPictureAsync(int categoryId)
        {
            var category = await this.context.Categories.FindAsync(categoryId).ConfigureAwait(false);
            if (category is null)
            {
                return false;
            }

            category.Picture = null;
            return true;
        }

        /// <inheritdoc/>
        public async Task<(bool, byte[] bytes)> TryGetPictureAsync(int categoryId)
        {
            var category = await this.context.Categories.FindAsync(categoryId).ConfigureAwait(false);
            if (category is null)
            {
                return (false, null);
            }

            return (category.Picture is not null, category.Picture);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePictureAsync(int categoryId, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var category = await this.context.Categories.FindAsync(categoryId).ConfigureAwait(false);
            if (category is null)
            {
                return false;
            }

            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            var bytes = memoryStream.ToArray();
            category.Picture = new byte[bytes.Length + ReservedBytes];

            bytes.CopyTo(category.Picture, ReservedBytes);
            this.context.Update(category);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            memoryStream.Dispose();
            return true;
        }
    }
}
