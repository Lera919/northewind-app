using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.DataAccess.Employees
{
    /// <summary>
    /// Represents a DAO for Northwind employees.
    /// </summary>
    public interface IEmployeeDataAccessObject
    {
        /// <summary>
        /// Inserts a new Northwind employee to a data storage.
        /// </summary>
        /// <param name="employee">A <see cref="EmployeeTransferObject"/>.</param>
        /// <returns>A data storage identifier of a new product.</returns>
        Task<int> InsertEmploteeAsync(EmployeeTransferObject employee);

        /// <summary>
        /// Deletes a Northwind employee from a data storage.
        /// </summary>
        /// <param name="employeeId">An product identifier.</param>
        /// <returns>True if a product is deleted; otherwise false.</returns>
        Task<bool> DeleteEmploteeAsync(int employeeId);

        /// <summary>
        /// Updates a Northwind employee in a data storage.
        /// </summary>
        /// <param name="employee">A <see cref="EmployeeTransferObject"/>.</param>
        /// <returns>True if a product is updated; otherwise false.</returns>
        Task<bool> UpdateEmploteeAsync(EmployeeTransferObject employee);

        /// <summary>
        /// Finds a Northwind employee using a specified identifier.
        /// </summary>
        /// <param name="employeeId">A data storage identifier of an existed product.</param>
        /// <returns>A <see cref="EmployeeTransferObject"/> with specified identifier.</returns>
        Task<EmployeeTransferObject> FindEmploteeAsync(int employeeId);

        /// <summary>
        /// Selects employees using specified offset and limit.
        /// </summary>
        /// <param name="offset">An offset of the first object.</param>
        /// <param name="limit">A limit of returned objects.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="EmployeeTransferObject"/>.</returns>
        IAsyncEnumerable<EmployeeTransferObject> SelectEmploteeAsync(int offset, int limit);

        /// <summary>
        /// Selects all Northwind employees with specified names.
        /// </summary>
        /// <param name="names">A <see cref="IEnumerable{T}"/> of product names.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="EmployeeTransferObject"/>.</returns>
        IAsyncEnumerable<EmployeeTransferObject> SelectEmploteeByNameAsync(ICollection<string> names);
    }
}
