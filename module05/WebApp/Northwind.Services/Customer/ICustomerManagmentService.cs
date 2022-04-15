using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Services
{
    public interface ICustomerManagmentService
    {

        /// <summary>
        /// Try to show a customer with specified identifier.
        /// </summary>
        /// <param name="customerId">A product category identifier.</param>
        /// <returns>Returns true if a product category is returned; otherwise false.</returns>
        Task<(bool result, Customer customer)> TryGetCustomerAsync(string customerId);

        /// <summary>
        /// Creates a new customer.
        /// </summary>
        /// <param name="customer">A <see cref="Customer"/> to create.</param>
        /// <returns>An identifier of a created product category.</returns>
        Task<string> CreateCustomerAsync(Customer customer);

        /// <summary>
        /// Destroys an existed customer.
        /// </summary>
        /// <param name="employeeId">A customer identifier.</param>
        /// <returns>True if a customer is destroyed; otherwise false.</returns>
        Task<bool> DestroyCustomerAsync(string customerId);

        Task<(bool, Customer)> TryGetByCompanyNameAsync(string name);


    }
}
