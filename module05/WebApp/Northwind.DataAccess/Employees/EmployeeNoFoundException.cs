using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.DataAccess.Employees
{
    /// <summary>
    /// EmployeeNoFoundException.
    /// </summary>
#pragma warning disable CA1032 // Implement standard exception constructors
    public class EmployeeNoFoundException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNoFoundException"/> class with specified identifier and object type.
        /// </summary>
        /// <param name="id">A requested identifier.</param>
        public EmployeeNoFoundException(int id)
            : base(string.Format(CultureInfo.InvariantCulture, $"An employee with identifier = {id}."))
        {
            this.EmployeeId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNoFoundException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">An object that describes the source or destination of the serialized data.</param>
        protected EmployeeNoFoundException(SerializationInfo info, StreamingContext context)
              : base(info, context)
        {
        }

        /// <summary>
        /// Gets an identifier of a product category that is missed in a data storage.
        /// </summary>
        public int EmployeeId { get; }
    }
}
