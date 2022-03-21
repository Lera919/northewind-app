using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services
{
    /// <summary>
    /// Provides access to photo in data storage.
    /// </summary>
    public interface IPhotoManagamentService
    {
        /// <summary>
        /// Try to show a product category picture.
        /// </summary>
        /// <param name="id">A product category identifier.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<(bool, byte[] bytes)> TryGetPhotoAsync(int id);

        /// <summary>
        /// Update a product category picture.
        /// </summary>
        /// <param name="id">An employee identifier.</param>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<bool> UpdatePhotoAsync(int id, Stream stream);

        /// <summary>
        /// Destroy a product category picture.
        /// </summary>
        /// <param name="id">An employee identifier.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<bool> DestroyPhotoAsync(int id);
    }
}
