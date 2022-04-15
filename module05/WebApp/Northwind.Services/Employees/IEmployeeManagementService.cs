using System.Collections.Generic;
using System.Threading.Tasks;
using WebAppModule6.Entities;

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public interface IEmployeeManagementService
    {
        /// <summary>
        /// Shows a list of product categories using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="EmployeeEntity"/>.</returns>
        IAsyncEnumerable<Employee> GetEmployeeAsync(int offset, int limit);

        /// <summary>
        /// Try to show a product category with specified identifier.
        /// </summary>
        /// <param name="employeeId">A product category identifier.</param>
        /// <returns>Returns true if a product category is returned; otherwise false.</returns>
        Task<(bool result, Employee employee)> TryGetEmployeeAsync(int employeeId);

        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="employee">A <see cref="EmployeeEntity"/> to create.</param>
        /// <returns>An identifier of a created product category.</returns>
        Task<int> CreateEmployeeAsync(Employee employee);

        /// <summary>
        /// Destroys an existed product category.
        /// </summary>
        /// <param name="employeeId">A product category identifier.</param>
        /// <returns>True if a product category is destroyed; otherwise false.</returns>
        Task<bool> DestroyEmployeeAsync(int employeeId);

        /// <summary>
        /// Looks up for product categories with specified names.
        /// </summary>
        /// <param name="names">A list of product category names.</param>
        /// <returns>A list of product categories with specified names.</returns>
        IAsyncEnumerable<Employee> LookupEmployeeByNameAsync(IList<string> names);

        /// <summary>
        /// Updates a product category.
        /// </summary>
        /// <param name="employeeId">A product category identifier.</param>
        /// <param name="employee">A <see cref="EmployeeEntity"/>.</param>
        /// <returns>True if a product category is updated; otherwise false.</returns>
        Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee);

        Task<(bool, Employee)> TryGetByNameAsync(string name);
    }
}
