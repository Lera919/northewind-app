// <copyright file="PhotoManagmentService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WebAppModule6.Context;

    /// <summary>
    /// PhotoManagmentService gets employees photoes.
    /// </summary>
    public class PhotoManagmentService : IPhotoManagamentService
    {
        private readonly NorthwindContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoManagmentService"/> class.
        /// </summary>
        /// <param name="context">Northwind context.</param>
        public PhotoManagmentService(NorthwindContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPhotoAsync(int id)
        {
            var employee = await this.context.Employees.FindAsync(id).ConfigureAwait(false);
            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;
            return true;
        }

        /// <inheritdoc/>
        public async Task<(bool, byte[] bytes)> TryGetPhotoAsync(int id)
        {
            var employee = await this.context.Employees.FindAsync(id).ConfigureAwait(false);
            if (employee is null)
            {
                return (false, null);
            }

            return (employee.Photo is not null, employee.Photo);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePhotoAsync(int id, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var employee = await this.context.Employees.FindAsync(id).ConfigureAwait(false);
            if (employee is null)
            {
                return false;
            }

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            var bytes = memoryStream.ToArray();
            bytes.CopyTo(employee.Photo, 78);
            this.context.Update(employee);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
    }
}
