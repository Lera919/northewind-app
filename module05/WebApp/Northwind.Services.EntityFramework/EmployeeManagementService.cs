// <copyright file="EmployeeManagementService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.Services.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Bogus;
    using Microsoft.EntityFrameworkCore;
    using Northwind.DataAccess;
    using Northwind.Services.EntityFrameworkCore.Blogging;
    using NorthwindWebApp.Context;
    using NorthwindWebApp.Entities;

    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly NorthwindContext context;
        private readonly BloggingContext bloggingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="contex">Northwind context.</param>
        /// <param name="bloggingContext">Blogging context.</param>
        public EmployeeManagementService(NorthwindContext contex, BloggingContext bloggingContext)
        {
            this.context = contex;
            this.bloggingContext = bloggingContext;
        }

        /// <inheritdoc/>
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            this.context.Employees.Add(employee);
            int rowsAffected = await this.context.SaveChangesAsync().ConfigureAwait(false);
            return rowsAffected;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            var (result, employee) = await this.TryGetEmployeeAsync(employeeId).ConfigureAwait(false);
            if (result)
            {
                this.context.EmployeeTerritories.RemoveRange(this.context.EmployeeTerritories.Where(terr => terr.EmployeeId == employeeId));
                this.context.Employees.Remove(employee);
                var employeesArticles = this.bloggingContext.Articles.Where(article => article.AuthorId == employeeId);
                this.bloggingContext.RemoveRange(employeesArticles);
                await this.context.SaveChangesAsync().ConfigureAwait(false);
                await this.bloggingContext.SaveChangesAsync().ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Employee> GetEmployeeAsync(int offset, int limit)
         => this.context.Employees.Skip(offset).Take(limit).OrderBy(em => em.EmployeeId).AsAsyncEnumerable();

        /// <inheritdoc/>
        public IAsyncEnumerable<Employee> LookupEmployeeByNameAsync(IList<string> names)
        => this.context.Employees.Where(em => names.Contains(em.FirstName)).OrderBy(em => em.EmployeeId).AsAsyncEnumerable();

        /// <inheritdoc/>
        public async Task<(bool result, Employee employee)> TryGetEmployeeAsync(int employeeId)
        {
            var employee = await this.context.Employees.FindAsync(employeeId).ConfigureAwait(false);
            return (employee != null, employee);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            var toUpdate = await this.context.Employees.FindAsync(employeeId).ConfigureAwait(false);
            if (toUpdate is null)
            {
                return false;
            }
            else
            {
                employee.EmployeeId = employeeId;
                this.context.Employees.Update(employee);
                await this.context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
        }
    }
}
