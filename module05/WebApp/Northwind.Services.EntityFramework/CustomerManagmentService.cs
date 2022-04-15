using AutoMapper;
using Northwind.Services;
using Northwind.Services.EntityFrameworkCore.Blogging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppModule6.Context;
using WebAppModule6.Entities;

namespace Northwind.Services.EntityFramework
{
    /// <inheritdoc/>
    public class CustomerManagmentService : ICustomerManagmentService
    {
        private readonly NorthwindContext context;
        private readonly IMapper mapper;
        private readonly BloggingContext bloggingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerManagmentService"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="bloggingContext">Blogging context.</param>
        /// <param name="mapper">Mapper.</param>
        public CustomerManagmentService(NorthwindContext context, BloggingContext bloggingContext, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.bloggingContext = bloggingContext;
        }

        /// <inheritdoc/>
        public async Task<string> CreateCustomerAsync(Customer customer)
        {
            var createdCustomer = this.context.Customers.Add(this.mapper.Map<CustomerEntity>(customer)).Entity;
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            var createdEmployeeid = createdCustomer.CustomerId;
            return createdEmployeeid;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCustomerAsync(string customerId)
        {
            var customer = await this.context.Employees.FindAsync(customerId).ConfigureAwait(false);
            var result = customer is not null;
            if (result)
            {
                this.context.CustomerCustomerDemos.RemoveRange(this.context.CustomerCustomerDemos.Where(terr => terr.CustomerId == customerId));
                this.context.Customers.Remove(this.mapper.Map<CustomerEntity>(customer));
                var comments = this.bloggingContext.Comments.Where(article => article.CustomerId == customerId);
                this.bloggingContext.RemoveRange(comments);
                await this.context.SaveChangesAsync().ConfigureAwait(false);
                await this.bloggingContext.SaveChangesAsync().ConfigureAwait(false);
            }

            return result;
        }

        public async Task<(bool, Customer)> TryGetByCompanyNameAsync(string name)
        {
            var customer = this.mapper.Map<Customer>(this.context.Customers.SingleOrDefault(x => x.CompanyName == name));
            return (customer is not null, customer);
        }

        /// <inheritdoc/>
        public async Task<(bool result, Customer customer)> TryGetCustomerAsync(string customerId)
        {
            var employee = await this.context.Customers.FindAsync(customerId).ConfigureAwait(false);
            return (employee != null, this.mapper.Map<Customer>(employee));
        }
    }
}
