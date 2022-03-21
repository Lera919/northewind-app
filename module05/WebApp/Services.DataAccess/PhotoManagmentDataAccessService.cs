using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.DataAccess;

namespace Northwind.Services.DataAccess
{
    public class PhotoManagmentDataAccessService : IPhotoManagamentService
    {
        public PhotoManagmentDataAccessService(NorthwindDataAccessFactory factory)
        {
            this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            //this.maxFileSixe = maxSize;
        }

        private NorthwindDataAccessFactory Factory { get; set; }
        // private readonly long maxFileSixe;
        private const int ReservedBytesLength = 78;

        public async Task<(bool, byte[] bytes)> TryGetPhotoAsync(int id)
        {
            var employee = await this.Factory.GetEmployeeDataAccessObject().FindEmploteeAsync(id);
            var photo = employee.Photo is null ? null : employee.Photo;
            return (photo is not null, employee.Photo);
        }

        public async Task<bool> UpdatePhotoAsync(int id, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            //if(stream.Length > this.maxFileSixe)
            //{
            //    throw new ArgumentException("File is too long", nameof(stream));
            //}
            var employee = await this.Factory.GetEmployeeDataAccessObject().FindEmploteeAsync(id);
            await using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            byte[] bytes = memory.ToArray();
            bytes.CopyTo(employee.Photo, ReservedBytesLength);
            return await this.Factory.GetEmployeeDataAccessObject().UpdateEmploteeAsync(employee);

        }

        public async Task<bool> DestroyPhotoAsync(int id)
        {
            var employee = await this.Factory.GetEmployeeDataAccessObject().FindEmploteeAsync(id);
            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;
            return true;
        }
    }
}
