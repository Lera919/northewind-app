using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Northwind.DataAccess;

namespace Northwind.Services.DataAccess
{
    public class PictureManagmentDataAccessService : IProductPictureManagementService
    {

        public PictureManagmentDataAccessService(NorthwindDataAccessFactory factory)
        {
            this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            //this.maxFileSixe = maxSize;
        }

        private NorthwindDataAccessFactory Factory { get; set; }
        // private readonly long maxFileSixe;
        private const int ReservedBytesLength = 78;
        public async Task<bool> DestroyPictureAsync(int categoryId)
        {
            var category = await this.Factory.GetProductCategoryDataAccessObject().FindProductCategoryAsync(categoryId);
            if (category is null)
            {
                return false;
            }

            category.Picture = null;
            return true;
        }

        public async Task<(bool, byte[] bytes)> TryGetPictureAsync(int categoryId)
        {
            var category = await this.Factory.GetProductCategoryDataAccessObject().FindProductCategoryAsync(categoryId);
            var picture = category.Picture is null ? null : category.Picture;
            return (picture is not null, category.Picture);
        }

        public async Task<bool> UpdatePictureAsync(int categoryId, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            //if(stream.Length > this.maxFileSixe)
            //{
            //    throw new ArgumentException("File is too long", nameof(stream));
            //}
            var category = await this.Factory.GetProductCategoryDataAccessObject().FindProductCategoryAsync(categoryId);
            await using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            byte[] bytes = memory.ToArray();
            bytes.CopyTo(category.Picture, ReservedBytesLength);
            return await this.Factory.GetProductCategoryDataAccessObject().UpdateProductCategoryAsync(category);
        }
    }
}
